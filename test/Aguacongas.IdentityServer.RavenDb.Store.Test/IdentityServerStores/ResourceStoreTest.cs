// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class ResourceStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameter()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceStore(null));
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_should_find_api_by_name()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.ProtectResource
            {
                Id = "test",
                ApiClaims = new List<Entity.ApiClaim> 
                {
                    new Entity.ApiClaim
                    {
                        Id = $"{nameof(Entity.ApiClaim).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiClaim
            {
                Id = "test",
                ApiId = "test",
                Type = "test"
            }, $"{nameof(Entity.ApiClaim).ToLowerInvariant()}/test");
            await s1.SaveChangesAsync();

            var sut = new ResourceStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()));

            var result = await sut.FindApiResourcesByNameAsync(new[] { "test" });

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_should_find_api_by_scope_name()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.ProtectResource
            {
                Id = "test",
                ApiScopes = new List<Entity.ApiApiScope>
                {
                    new Entity.ApiApiScope
                    {
                        Id = $"{nameof(Entity.ApiApiScope).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiApiScope
            {
                Id = "test",
                ApiId = "test",
                ApiScopeId = "test"
            }, $"{nameof(Entity.ApiApiScope).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiScope
            {
                Id = "test",
                Apis = new List<Entity.ApiApiScope>
                {
                    new Entity.ApiApiScope
                    {
                        Id = $"{nameof(Entity.ApiApiScope).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.ApiScope).ToLowerInvariant()}/test");
            await s1.SaveChangesAsync();

            var sut = new ResourceStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()));

            var result = await sut.FindApiResourcesByScopeNameAsync(new[] { "test" });

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_should_find_apiScope_by_name()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.ApiScope
            {
                Id = "test",
                Properties = new List<Entity.ApiScopeProperty>
                {
                    new Entity.ApiScopeProperty
                    {
                        Id = $"{nameof(Entity.ApiScopeProperty).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.ApiScope).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiScopeProperty
            {
                Id = "test",
                ApiScopeId = "test",
                Key = "test",
                Value = "test"
            }, $"{nameof(Entity.ApiScopeProperty).ToLowerInvariant()}/test");
            await s1.SaveChangesAsync();

            var sut = new ResourceStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()));

            var result = await sut.FindApiScopesByNameAsync(new[] { "test" });

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_should_find_identity_by_name()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.IdentityResource
            {
                Id = "test",
                Resources = new List<Entity.IdentityLocalizedResource>
                {
                    new Entity.IdentityLocalizedResource
                    {
                        Id = $"{nameof(Entity.IdentityLocalizedResource).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.IdentityResource).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.IdentityLocalizedResource
            {
                Id = "test",
                IdentityId = "test",
                ResourceKind = Entity.EntityResourceKind.DisplayName,
                Value = "test"
            }, $"{nameof(Entity.IdentityLocalizedResource).ToLowerInvariant()}/test");
            await s1.SaveChangesAsync();

            var sut = new ResourceStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()));

            var result = await sut.FindIdentityResourcesByScopeNameAsync(new[] { "test" });

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllResourcesAsync_should_return_all_resources()
        {
            var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.ProtectResource
            {
                Id = "test",
                ApiScopes = new List<Entity.ApiApiScope>
                {
                    new Entity.ApiApiScope
                    {
                        Id = $"{nameof(Entity.ApiApiScope).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.ProtectResource).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiApiScope
            {
                Id = "test",
                ApiId = "test",
                ApiScopeId = "test"
            }, $"{nameof(Entity.ApiApiScope).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.ApiScope
            {
                Id = "test",
                Apis = new List<Entity.ApiApiScope>
                {
                    new Entity.ApiApiScope
                    {
                        Id = $"{nameof(Entity.ApiApiScope).ToLowerInvariant()}/test"
                    }
                }
            }, $"{nameof(Entity.ApiScope).ToLowerInvariant()}/test");
            await s1.StoreAsync(new Entity.IdentityResource
            {
                Id = "test",
            }, $"{nameof(Entity.IdentityResource).ToLowerInvariant()}/test");
            await s1.SaveChangesAsync();

            var sut = new ResourceStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()));

            var result = await sut.GetAllResourcesAsync();

            Assert.NotEmpty(result.IdentityResources);
            Assert.NotEmpty(result.ApiResources);
            Assert.NotEmpty(result.ApiScopes);
        }
    }
}
