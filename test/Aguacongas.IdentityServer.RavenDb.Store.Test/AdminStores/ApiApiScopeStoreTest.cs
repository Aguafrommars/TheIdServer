// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores
{
    public class ApiApiScopeStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new ApiApiScopeStore(null, null));
            Assert.Throws<ArgumentNullException>(() => new ApiApiScopeStore(new RavenDbTestDriverWrapper().GetDocumentStore().OpenAsyncSession(), null));
        }

        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parents()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ProtectResource
            {
                Id = "test"
            }, $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiScope
            {
                Id = "test"
            }, $"{nameof(Entity.ApiScope).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<ApiApiScopeStore>>();

            using var session = store.OpenAsyncSession();

            var sut = new ApiApiScopeStore(session, loggerMock.Object);

            var entity = new Entity.ApiApiScope
            {
                Id = Guid.NewGuid().ToString(),
                ApiId = "test",
                ApiScopeId = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var api = await s2.LoadAsync<Entity.ProtectResource>($"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");
            Assert.Contains(api.ApiScopes, i => i.Id == $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}");
            var apiScope = await s2.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/test");
            Assert.Contains(apiScope.Apis, i => i.Id == $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parents_not_found()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ProtectResource
            {
                Id = "test"
            }, $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<ApiApiScopeStore>>();

            using var session = store.OpenAsyncSession();

            var sut = new ApiApiScopeStore(session, loggerMock.Object);

            var entity = new Entity.ApiApiScope
            {
                Id = Guid.NewGuid().ToString(),
                ApiId = "unknow",
                ApiScopeId = "test"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));

            entity.ApiId = "test";
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_should_remove_entity_id_from_parents()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.ApiApiScope
            {
                Id = Guid.NewGuid().ToString(),
                ApiId = "test",
                ApiScopeId = "test"
            };
            var api = new Entity.ProtectResource
            {
                Id = "test",
                ApiScopes = new List<Entity.ApiApiScope>
                {
                    new Entity.ApiApiScope
                    {
                        Id = $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}"
                    }
                }
            };
            var apiScope = new Entity.ApiScope
            {
                Id = "test",
                Apis = new List<Entity.ApiApiScope>
                {
                    new Entity.ApiApiScope
                    {
                        Id = $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}"
                    }
                }
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(api, $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{api.Id}");
            await s1.StoreAsync(apiScope, $"{nameof(Entity.ApiScope).ToLowerInvariant()}/{apiScope.Id}");
            await s1.StoreAsync(entity, $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.ApiApiScope>>>();

            using var session = store.OpenAsyncSession();

            var sut = new ApiApiScopeStore(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var apiUpdated = await s2.LoadAsync<Entity.ProtectResource>($"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");
            Assert.DoesNotContain(apiUpdated.ApiScopes, i => i.Id == $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}");
            var apiScopeUpdated = await s2.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/test");
            Assert.DoesNotContain(apiScopeUpdated.Apis, i => i.Id == $"{typeof(Entity.ApiApiScope).Name.ToLowerInvariant()}/{entity.Id}");
        }
    }
}
