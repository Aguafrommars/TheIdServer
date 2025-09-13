// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Test;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.Test
{
    public class AdminStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameter()
        {
            Assert.Throws<ArgumentNullException>(() => new AdminStore<ProtectResource>(null, null));
            Assert.Throws<ArgumentNullException>(() => new AdminStore<ProtectResource>(new ScopedAsynDocumentcSession(new RavenDbTestDriverWrapper().GetDocumentStore().OpenAsyncSession()), null));
        }

        [Fact]
        public async Task GetAsync_should_filter_by_odata_request()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ProtectResource>>>();

            var id = "test";
            using (var s1 = store.OpenAsyncSession())
            {                
                await s1.StoreAsync(new ProtectResource
                {
                    Id = id,
                    Description = "test",
                    DisplayName = "test",
                    Enabled = true
                }, $"{nameof(ProtectResource).ToLowerInvariant()}/{id}");
                await s1.SaveChangesAsync();
            }

            using var session = store.OpenAsyncSession();

            var sut = new AdminStore<ProtectResource>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            var page = await sut.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ProtectResource.Id)} eq 'test'",
                Skip = 0,
                Take = 10
            });

            Assert.Single(page.Items);

            page = await sut.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ProtectResource.DisplayName)} eq 'no-test'",
                Skip = 0,
                Take = 10
            });

            Assert.Empty(page.Items);

            page = await sut.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ProtectResource.CreatedAt)} eq {DateTime.UtcNow.ToString("o")}",
                Skip = 0,
                Take = 10
            });

            Assert.Empty(page.Items);
        }

        [Fact]
        public async Task GetAsync_by_odata_requqest_should_expand_properties()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ProtectResource>>>();

            var id = "test";
            var resourceId = Guid.NewGuid().ToString();

            using (var s1 = store.OpenAsyncSession())
            {                
                await s1.StoreAsync(new ProtectResource
                {
                    Id = id,
                    Description = "test",
                    DisplayName = "test",
                    Enabled = true,
                    Resources = new List<ApiLocalizedResource> 
                    { 
                        new ApiLocalizedResource
                        {
                            Id = $"{nameof(ApiLocalizedResource).ToLowerInvariant()}/{resourceId}"
                        }
                    }
                }, $"{nameof(ProtectResource).ToLowerInvariant()}/{id}");

                
                await s1.StoreAsync(new ApiLocalizedResource
                {
                    Id = resourceId,
                    ApiId = $"{nameof(ApiLocalizedResource).ToLowerInvariant()}/{id}",
                    CultureId = "fr",
                    ResourceKind = EntityResourceKind.DisplayName,
                    Value = "test"
                }, $"{nameof(ApiLocalizedResource).ToLowerInvariant()}/{resourceId}");

                await s1.SaveChangesAsync();
            }

            using var session = store.OpenAsyncSession();

            var sut = new AdminStore<ProtectResource>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            var page = await sut.GetAsync(new PageRequest
            {
                Expand = "Resources"
            });

            Assert.Single(page.Items);
            Assert.NotNull(page.Items.First().Resources);
            Assert.Single(page.Items.First().Resources);
            Assert.Equal(resourceId, page.Items.First().Resources.First().Id);
            Assert.Equal(id, page.Items.First().Resources.First().ApiId);
            Assert.Equal("fr", page.Items.First().Resources.First().CultureId);
            Assert.Equal("test", page.Items.First().Resources.First().Value);
        }

        [Fact]
        public async Task GetAsync_by_id_should_expand_properties()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ApiClaim>>>();

            var id = "test";
            var claimId = Guid.NewGuid().ToString();
            using (var s1 = store.OpenAsyncSession())
            {
                
                await s1.StoreAsync(new ProtectResource
                {
                    Id = id,
                    Description = "test",
                    DisplayName = "test",
                    Enabled = true,
                    ApiClaims = new List<ApiClaim>
                    {
                        new ApiClaim
                        {
                            Id = $"{nameof(ApiClaim).ToLowerInvariant()}/{claimId}"
                        }
                    }
                }, $"{nameof(ProtectResource).ToLowerInvariant()}/{id}");


                await s1.StoreAsync(new ApiClaim
                {
                    Id = claimId,
                    ApiId = $"{nameof(ProtectResource).ToLowerInvariant()}/{id}",
                    Type = "test"
                }, $"{nameof(ApiClaim).ToLowerInvariant()}/{claimId}");

                await s1.SaveChangesAsync();
            }

            using var session = store.OpenAsyncSession();

            var sut = new AdminStore<ApiClaim>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            var claim = await sut.GetAsync(claimId, new GetRequest
            {
                Expand = "Api"
            });

            Assert.NotNull(claim);
            Assert.NotNull(claim.Api);
            Assert.Equal(id, claim.Api.Id);
            Assert.Equal(id, claim.ApiId);
        }

        [Fact]
        public async Task GetAsync_by_id_should_throw_on_expand_wrong_property()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ApiClaim>>>();


            using var session = store.OpenAsyncSession();

            var sut = new AdminStore<ApiClaim>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetAsync("test", new GetRequest
            {
                Expand = "Type"
            }));

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetAsync("test", new GetRequest
            {
                Expand = "Test"
            }));
        }

        [Fact]
        public async Task DeleteAsync_should_delete_cascade()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ProtectResource>>>();

            var id = "test";
            var claimId = Guid.NewGuid().ToString();
            using (var s1 = store.OpenAsyncSession())
            {
                
                await s1.StoreAsync(new ProtectResource
                {
                    Id = id,
                    Description = "test",
                    DisplayName = "test",
                    Enabled = true,
                    ApiClaims = new List<ApiClaim>
                    {
                        new ApiClaim
                        {
                            Id = $"{nameof(ApiClaim).ToLowerInvariant()}/{claimId}"
                        }
                    }
                }, $"{nameof(ProtectResource).ToLowerInvariant()}/{id}");


                await s1.StoreAsync(new ApiClaim
                {
                    Id = claimId,
                    ApiId = $"{nameof(ProtectResource).ToLowerInvariant()}/{id}",
                    Type = "test"
                }, $"{nameof(ApiClaim).ToLowerInvariant()}/{claimId}");

                await s1.SaveChangesAsync();
            }

            using (var session = store.OpenAsyncSession())
            {
                var sut = new AdminStore<ProtectResource>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

                await sut.DeleteAsync(id);
            }


            using var s2 = store.OpenAsyncSession();

            var claimStore = new AdminStore<ApiClaim>(new ScopedAsynDocumentcSession(s2), new Mock<ILogger<AdminStore<ApiClaim>>>().Object);
            var claim = await claimStore.GetAsync(claimId, null);
            Assert.Null(claim);
        }

        [Fact]
        public async Task DeleteAsync_should_throw_on_entity_not_found()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ProtectResource>>>();

            var id = "test";

            using var session = store.OpenAsyncSession();
            var sut = new AdminStore<ProtectResource>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteAsync(id));
        }

        [Fact]
        public async Task CreateAsync_should_populate_auditable_properties_when_auditable()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<TestEntity>>>();

            using var session = store.OpenAsyncSession();
            var sut = new AdminStore<TestEntity>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            var result = await sut.CreateAsync(new TestEntity() as object) as TestEntity;

            Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task UpdateAsync_should_populate_auditable_properties_when_auditable()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<TestEntity>>>();

            using var session = store.OpenAsyncSession();
            var sut = new AdminStore<TestEntity>(new ScopedAsynDocumentcSession(session), loggerMock.Object);

            var entity = new TestEntity();
            entity = await sut.CreateAsync(entity);

            await sut.UpdateAsync(entity);

            Assert.NotNull(entity.Id);
        }

        public class TestEntity : IEntityId
        {
            public string Id { get; set; }
        }
    }
}
