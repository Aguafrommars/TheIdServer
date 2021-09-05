// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Models;
using AutoMapper.Internal;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.MongoDb.Store.Test.AdminStores
{
    public abstract class AdminStoreTestBase<TEntity> : IDisposable
        where TEntity: class, IEntityId, new()
    {
        private bool disposedValue;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        protected AdminStoreTestBase()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb://localhost");
            _client = new MongoClient(settings);
            _database = _client.GetDatabase(Guid.NewGuid().ToString());
        }

        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new AdminStore<TEntity>(null, null));
            Assert.Throws<ArgumentNullException>(() => new AdminStore<TEntity>(new Mock<IServiceProvider>().Object, null));
        }

        [Fact]
        public async Task GetAsync_by_page_request_should_expand_sur_entities()
        {
            var navigationProperties = GetNavigrationProperties();
            var services = new ServiceCollection()
                .AddLogging()
                .Configure<MemoryCacheOptions>(options => { })
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();
            var provider = services
                .AddTheIdServerMongoDbStores(p => _database)
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<IAdminStore<TEntity>>();
            Assert.NotNull(sut);

            await CreateEntityGraphAsync(navigationProperties, provider, sut).ConfigureAwait(false);

            var entities = await sut.GetAsync(new PageRequest
            {
                Expand = GetExpand()
            }).ConfigureAwait(false);

            Assert.NotEmpty(entities.Items);
            foreach(var property in navigationProperties)
            {
                Assert.Contains(entities.Items, i => property.GetValue(i) != null);
            }
        }

        [Fact]
        public async Task GetAsync_by_id_should_expand_sur_entities()
        {
            var navigationProperties = GetNavigrationProperties();
            var services = new ServiceCollection()
                .AddLogging()
                .Configure<MemoryCacheOptions>(options => { })
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();
            var provider = services
                .AddTheIdServerMongoDbStores(p => _database)
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<IAdminStore<TEntity>>();
            Assert.NotNull(sut);
            var entity = await CreateEntityGraphAsync(navigationProperties, provider, sut).ConfigureAwait(false);

            var result = await sut.GetAsync(entity.Id, new GetRequest
            {
                Expand = GetExpand()
            }).ConfigureAwait(false);

            Assert.NotNull(result);
            foreach (var property in navigationProperties.Where(p => p.PropertyType.ImplementsGenericInterface(typeof(ICollection<>))))
            {
                Assert.NotEmpty(property.GetValue(result) as IEnumerable);
            }
            foreach (var property in navigationProperties.Where(p => !p.PropertyType.ImplementsGenericInterface(typeof(ICollection<>))))
            {
                Assert.NotNull(property.GetValue(result));
            }
        }

        [Fact]
        public async Task UpdateAsync_should_update_entity()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .Configure<MemoryCacheOptions>(options => { })
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();
            var provider = services
                .AddTheIdServerMongoDbStores(p => _database)
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<IAdminStore<TEntity>>();
            Assert.NotNull(sut);

            var create = new TEntity();
            var entity = await sut.CreateAsync(create).ConfigureAwait(false);
            await sut.UpdateAsync(entity).ConfigureAwait(false);

            var updated = await sut.GetAsync(entity.Id, null).ConfigureAwait(false);
            if (updated is IAuditable auditable)
            {
                Assert.NotNull(auditable.ModifiedAt);
            }            
        }

        [Fact]
        public async Task DeleteAsync_should_cascade_delete()
        {
            var navigationProperties = GetNavigrationProperties();
            var services = new ServiceCollection()
                .AddLogging()
                .Configure<MemoryCacheOptions>(options => { })
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();
            var provider = services
                .AddTheIdServerMongoDbStores(p => _database)
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<IAdminStore<TEntity>>();
            Assert.NotNull(sut);
            
            var entity = await CreateEntityGraphAsync(navigationProperties, provider, sut).ConfigureAwait(false);

            await sut.DeleteAsync(entity.Id).ConfigureAwait(false);

            var nullResult = await sut.GetAsync(entity.Id, null).ConfigureAwait(false);
            Assert.Null(nullResult);
            foreach (var property in navigationProperties.Where(p => p.PropertyType.ImplementsGenericInterface(typeof(ICollection<>))))
            {
                var subEntityType = property.PropertyType.GetGenericArguments()[0];
                var storeType = typeof(IAdminStore<>).MakeGenericType(subEntityType);
                var subStore = provider.GetRequiredService(storeType) as IAdminStore;
                var getPageResponseMethod = storeType.GetMethod(nameof(IAdminStore<object>.GetAsync), new[] { typeof(PageRequest), typeof(CancellationToken) });
                var task = getPageResponseMethod.Invoke(subStore, new[]
                {
                    new PageRequest
                    {
                        Filter = $"{GetSubEntityParentIdName(typeof(TEntity))} eq '{entity.Id}'"
                    },
                    null
                });
                await (task as Task).ConfigureAwait(false);
                var response = task.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(task);
                var items = response.GetType().GetProperty(nameof(PageResponse<object>.Items)).GetValue(response);
                Assert.Empty(items as IEnumerable);
            }
        }

        [Fact]
        public async Task DeleteAsync_should_throw_on_entity_not_found()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .Configure<MemoryCacheOptions>(options => { })
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();
            var provider = services
                .AddTheIdServerMongoDbStores(p => _database)
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<IAdminStore<TEntity>>();
            Assert.NotNull(sut);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(Guid.NewGuid().ToString())).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateAsync_should_throw_on_entity_not_found()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .Configure<MemoryCacheOptions>(options => { })
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();
            var provider = services
                .AddTheIdServerMongoDbStores(p => _database)
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<IAdminStore<TEntity>>();
            Assert.NotNull(sut);

            var notFound = new TEntity();
            notFound.Id = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateAsync(notFound)).ConfigureAwait(false);
        }

        protected virtual object CreateParentEntiy(Type parentType)
        {
            return Activator.CreateInstance(parentType);
        }

        private async Task<TEntity> CreateEntityGraphAsync(IEnumerable<PropertyInfo> navigationProperties, ServiceProvider provider, IAdminStore<TEntity> sut)
        {
            var create = new TEntity
            {
                Id = Guid.NewGuid().ToString()
            };
            var parentPropetyName = GetParentIdName(create.GetType());
            if (parentPropetyName != null)
            {
                var parentPropetyType = GetParentType(parentPropetyName);
                await AddParentEntity(provider, create, parentPropetyName, parentPropetyType).ConfigureAwait(false);
            }
            var entity = await sut.CreateAsync(create).ConfigureAwait(false);
            foreach (var property in navigationProperties)
            {
                var subEntityType = property.PropertyType;
                if (subEntityType.ImplementsGenericInterface(typeof(ICollection<>)))
                {
                    subEntityType = subEntityType.GetGenericArguments()[0];
                    var parentPropety = subEntityType.GetProperty(GetSubEntityParentIdName(entity.GetType()));
                    var subEntity = Activator.CreateInstance(subEntityType);
                    parentPropety.SetValue(subEntity, entity.Id);

                    var storeType = typeof(IAdminStore<>).MakeGenericType(subEntityType);
                    var subStore = provider.GetRequiredService(storeType) as IAdminStore;
                    await subStore.CreateAsync(subEntity).ConfigureAwait(false);
                    continue;
                }

                await AddParentEntity(provider, create, $"{property.Name}Id", property.PropertyType).ConfigureAwait(false);
            }
            await sut.UpdateAsync(entity).ConfigureAwait(false);

            return entity;
        }

        private async Task AddParentEntity(ServiceProvider provider, TEntity create, string parentPropetyName, Type parentPropetyType)
        {
            var parentStoreType = typeof(IAdminStore<>).MakeGenericType(parentPropetyType);
            var parentStore = provider.GetRequiredService(parentStoreType) as IAdminStore;
            var parent = await parentStore.CreateAsync(CreateParentEntiy(parentPropetyType)).ConfigureAwait(false) as IEntityId;
            var parentPropety = create.GetType().GetProperty(parentPropetyName);
            parentPropety.SetValue(create, parent.Id);
        }

        private static Type GetParentType(string parentPropetyName)
        => parentPropetyName switch
            {
                nameof(IClientSubEntity.ClientId) => typeof(Client),
                nameof(IApiSubEntity.ApiId) => typeof(ProtectResource),
                nameof(IIdentitySubEntity.IdentityId) => typeof(IdentityResource),
                nameof(IUserSubEntity.UserId) => typeof(User),
                nameof(IRoleSubEntity.RoleId) => typeof(Role),
                _ => throw new InvalidOperationException(),
            };        

        private static string GetExpand()
        {
            return string.Join(',', GetNavigrationProperties().Select(p => p.Name));
        }

        private static IEnumerable<PropertyInfo> GetNavigrationProperties()
        {
            var properties = typeof(TEntity).GetProperties();
            var collectionProperties = properties
                .Where(p => p.PropertyType.ImplementsGenericInterface(typeof(ICollection<>)));
            var navigationProperties = properties
                .Where(p => properties.Any(e => e.Name == $"{p.Name}Id"));
            return collectionProperties.Concat(navigationProperties);
        }

        private static string GetSubEntityParentIdName(Type type)
            => $"{type.Name.Replace("ProtectResource", "Api").Replace("IdentityResource", "Identity")}Id";

        private static string GetParentIdName(Type type)
        {
            if (type.GetInterface(nameof(IClientSubEntity)) != null)
            {
                return nameof(IClientSubEntity.ClientId);
            }
            if (type.GetInterface(nameof(IApiSubEntity)) != null)
            {
                return nameof(IApiSubEntity.ApiId);
            }
            if (type.GetInterface(nameof(IIdentitySubEntity)) != null)
            {
                return nameof(IIdentitySubEntity.IdentityId);
            }
            if (type.GetInterface(nameof(IUserSubEntity)) != null)
            {
                return nameof(IUserSubEntity.UserId);
            }
            if (type.GetInterface(nameof(IRoleSubEntity)) != null)
            {
                return nameof(IRoleSubEntity.RoleId);
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.DropDatabase(_database.DatabaseNamespace.DatabaseName);
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
