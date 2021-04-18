// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using AutoMapper.Internal;
using Community.OData.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.MongoDb.Store
{
    public class AdminStore<TEntity> : IAdminStore<TEntity>
        where TEntity : class, IEntityId, new()
    {
        private readonly IServiceProvider _provider;
        private readonly IMongoCollection<TEntity> _collection;
        private readonly ILogger<AdminStore<TEntity>> _logger;
        private readonly Type _entityType = typeof(TEntity);
        public AdminStore(IServiceProvider provider, ILogger<AdminStore<TEntity>> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _collection = provider.GetRequiredService<IMongoCollection<TEntity>>();
        }

        public async Task<TEntity> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {            
            var entity = await _collection.AsQueryable()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                .ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }
            await AddExpandedAsync(entity, request?.Expand).ConfigureAwait(false);
            return entity;
        }

        public async Task<PageResponse<TEntity>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            var oDataQuery = _collection.AsQueryable().OData();
            oDataQuery = !string.IsNullOrWhiteSpace(request.Filter) ? oDataQuery.Filter(request.Filter) : oDataQuery;

            var query = oDataQuery.Inner as IMongoQueryable<TEntity>;

            var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var orderBy = request.OrderBy ?? nameof(IEntityId.Id);
            oDataQuery = oDataQuery.OrderBy(orderBy);
            query = oDataQuery.Inner as IMongoQueryable<TEntity>;

            if (request.Take.HasValue)
            {
                query = query.Skip(request.Skip ?? 0).Take(request.Take.Value);
            }

            var items = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

            
            foreach (var item in items)
            {
                await AddExpandedAsync(item, request.Expand).ConfigureAwait(false);
            }

            return new PageResponse<TEntity>
            {
                Count = count,
                Items = items
            };
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            using var session = await _collection.Database.Client.StartSessionAsync(cancellationToken: cancellationToken);

            var isTransactionSupported = session.Client.Cluster.Description.Type != ClusterType.Standalone;
            if (isTransactionSupported)
            {
                session.StartTransaction();
            }

            var entity = await _collection.AsQueryable()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} at id {id} is not found");
            }

            await _collection.DeleteOneAsync(session, Builders<TEntity>.Filter.Eq(e => e.Id, id), cancellationToken: cancellationToken).ConfigureAwait(false);
            
            var iCollectionType = typeof(ICollection<>);
            var iEntityIdType = typeof(IEntityId);
            var subEntitiesProperties = _entityType.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.ImplementsGenericInterface(iCollectionType) && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(iEntityIdType));

            var dataBase = _collection.Database;
            foreach (var subEntityProperty in subEntitiesProperties)
            {
                var subCollection = dataBase.GetCollection<BsonDocument>(subEntityProperty.PropertyType.GetGenericArguments()[0].Name);
                await subCollection.DeleteManyAsync(session, new BsonDocument(GetSubEntityParentIdName(_entityType), id), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (isTransactionSupported)
            {
                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Entity {EntityId} deleted", entity.Id, entity);
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            if (entity.Id == null)
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            if (entity is IAuditable auditable)
            {
                auditable.CreatedAt = DateTime.UtcNow;
            }

            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Entity {EntityId} created", entity.Id, entity);
            return entity;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as TEntity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var storedEntity = await _collection.AsQueryable()
                .FirstOrDefaultAsync(e => e.Id == entity.Id, cancellationToken)
                .ConfigureAwait(false);

            if (storedEntity == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} at id {entity.Id} is not found");
            }

            if (entity is IAuditable auditable)
            {
                auditable.ModifiedAt = DateTime.UtcNow;
            }
            var properties = typeof(TEntity).GetProperties()
                .Where(p => !p.PropertyType.ImplementsGenericInterface(typeof(ICollection<>)));
            foreach (var property in properties)
            {
                property.SetValue(storedEntity, property.GetValue(entity));
            }

            await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), storedEntity).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as TEntity, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task AddExpandedAsync(TEntity entity, string expand)
        {
            if (expand != null)
            {
                var pathList = expand.Split(',');
                foreach (var path in pathList)
                {
                    await PopulateSubEntitiesAsync(path, entity).ConfigureAwait(false);
                }
            }
        }

        private async Task PopulateSubEntitiesAsync(string path, TEntity entity)
        {
            var property = _entityType.GetProperty(path);
            var propertyType = property.PropertyType;
            
            if (propertyType.ImplementsGenericInterface(typeof(ICollection<>)))
            {
                var items = await GetSubItemsAsync(entity, propertyType).ConfigureAwait(false);
                property.SetValue(entity, items);
                return;
            }

            var navigationProperty = _entityType.GetProperty($"{path}Id");
            var parentId = navigationProperty.GetValue(entity) as string;
            var storeType = typeof(IAdminStore<>).MakeGenericType(propertyType);
            var subStore = _provider.GetRequiredService(storeType);
            var getMethod = storeType.GetMethod(nameof(IAdminStore<object>.GetAsync), new[] { typeof(string), typeof(GetRequest), typeof(CancellationToken) });

            var task = getMethod.Invoke(subStore, new[]
            {
                    parentId,
                    null,
                    null
                });
            await (task as Task).ConfigureAwait(false);
            var response = task.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(task);
            property.SetValue(entity, response);
        }

        private async Task<object> GetSubItemsAsync(TEntity entity, Type propertyType)
        {
            var storeType = typeof(IAdminStore<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
            var subStore = _provider.GetRequiredService(storeType);
            var getPageResponseMethod = storeType.GetMethod(nameof(IAdminStore<object>.GetAsync), new[] { typeof(PageRequest), typeof(CancellationToken) });
            var task = getPageResponseMethod.Invoke(subStore, new[]
            {
                    new PageRequest
                    {
                        Filter = $"{GetSubEntityParentIdName(_entityType)} eq '{entity.Id}'",
                        Take = null
                    },
                    null
                });
            await (task as Task).ConfigureAwait(false);
            var response = task.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(task);
            var items = response.GetType().GetProperty(nameof(PageResponse<object>.Items)).GetValue(response);
            return items;
        }

        private static string GetSubEntityParentIdName(Type type)
            => $"{type.Name.Replace("ProtectResource", "Api").Replace("IdentityResource", "Identity")}Id";

        
    }
}
