// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using AutoMapper.Internal;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class AdminStore<TEntity> : IAdminStore<TEntity>
        where TEntity : class, IEntityId, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<AdminStore<TEntity>> _logger;
        private readonly string _entitybasePath;
        private readonly Type _entityType = typeof(TEntity);
        public AdminStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<TEntity>> logger)
        {
            _session = session?.Session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var entityTypeName = typeof(TEntity).Name;
            _entitybasePath = entityTypeName.ToLowerInvariant() + "/";
        }

        public async Task<TEntity> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _session
                .LoadAsync<TEntity>($"{_entitybasePath}{id}", b => b.Expand(request?.Expand), cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }
            await AddExpandedAsync(entity, request?.Expand).ConfigureAwait(false);
            return entity;
        }

        public async Task<PageResponse<TEntity>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            var rql = request.ToRQL<TEntity, string>(_session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(TEntity)), e => e.Id);
            var pageQuery = _session.Advanced.AsyncRawQuery<TEntity>(rql);
            if (request.Take.HasValue)
            {
                pageQuery = pageQuery.GetPage(request);
            }

            var items = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            var countQuery = _session.Advanced.AsyncRawQuery<TEntity>(rql);
            var count = await countQuery.CountAsync(cancellationToken).ConfigureAwait(false);
            
            foreach(var item in items)
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
            var entity = await _session.LoadAsync<TEntity>($"{_entitybasePath}{id}", cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} at id {id} is not found");
            }
            _session.Delete(entity);
            var iCollectionType = typeof(ICollection<>);
            var iEntityIdType = typeof(IEntityId);
            var subEntitiesProperties = _entityType.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.ImplementsGenericInterface(iCollectionType) && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(iEntityIdType));
            foreach(var subEntityProperty in subEntitiesProperties)
            {
                var collection = subEntityProperty.GetValue(entity) as ICollection;
                if (collection != null)
                {
                    foreach (IEntityId subItem in collection)
                    {
                        _session.Delete(subItem.Id);
                    }
                }
            }

            await OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);

            await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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

            await OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);

            await _session.StoreAsync(entity, $"{_entitybasePath}{entity.Id}", cancellationToken).ConfigureAwait(false);
            await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
            var storedEntity = await _session.LoadAsync<TEntity>($"{_entitybasePath}{entity.Id}", cancellationToken).ConfigureAwait(false);

            if (storedEntity == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} at id {entity.Id} is not found");
            }
            if (entity is IAuditable auditable)
            {
                auditable.ModifiedAt = DateTime.UtcNow;
            }
            var properties = typeof(TEntity).GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(storedEntity, property.GetValue(entity));
            }

            await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as TEntity, cancellationToken)
                .ConfigureAwait(false);
        }

        protected virtual Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        private async Task AddExpandedAsync(TEntity entity, string expand)
        {
            if (expand != null)
            {
                var idName = _entityType.GetSubEntityParentIdName();
                var pathList = expand.Split(',');
                foreach (var path in pathList)
                {
                    await PopulateSubEntitiesAsync(idName, path, entity).ConfigureAwait(false);
                }
            }
        }

        private async Task PopulateSubEntitiesAsync(string idName, string path, TEntity entity)
        {
            var property = _entityType.GetProperty(path);
            var value = property.GetValue(entity);
            if (value == null)
            {
                if (property.PropertyType.ImplementsGenericInterface(typeof(ICollection<>)))
                {
                    value = Activator.CreateInstance(typeof(Collection<>).MakeGenericType(property.PropertyType.GetGenericArguments()));
                }
                else
                {
                    value = Activator.CreateInstance(property.PropertyType);
                }
                
                property.SetValue(entity, value);
            }

            if (value is ICollection collection)
            {
                foreach (var v in collection)
                {
                    await PopulateSubEntityAsync("Id", v).ConfigureAwait(false);
                    var idProperty = v.GetType().GetProperty(idName);
                    idProperty.SetValue(v, entity.Id);
                }
                return;
            }

            var parentIdProperty = _entityType.GetProperty($"{path}Id");
            ((IEntityId)value).Id = parentIdProperty.GetValue(entity) as string;
            await PopulateSubEntityAsync("Id", value).ConfigureAwait(false);
            parentIdProperty.SetValue(entity, ((IEntityId)value).Id);
        }

        private async Task PopulateSubEntityAsync(string idName, object subEntity)
        {
            var type = subEntity.GetType();
            var idProperty = type.GetProperty(idName);
            var id = idProperty.GetValue(subEntity);
            var loaded = await _session.LoadAsync<object>(id as string).ConfigureAwait(false);
            if (loaded == null)
            {
                return;
            }

            // remove navigation
            var iCollectionType = typeof(ICollection<>);
            var iEntityIdType = typeof(IEntityId);
            var subEntitiesProperties = type.GetProperties().Where(p => p.PropertyType.IsGenericType &&
                p.PropertyType.ImplementsGenericInterface(iCollectionType) &&
                p.PropertyType.GetGenericArguments()[0].IsAssignableTo(iEntityIdType));
            foreach (var subEntityProperty in subEntitiesProperties)
            {
                subEntityProperty.SetValue(loaded, null);
            }

            CloneEntity(subEntity, type, loaded);
            idProperty.SetValue(subEntity, ((IEntityId)loaded).Id);
        }

        private static void CloneEntity(object entity, Type type, object loaded)
        {
            foreach (var property in type.GetProperties(BindingFlags.Instance))
            {
                property.SetValue(entity, property.GetValue(loaded));
            }
        }
    }
}
