// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test
{
    public class AdminStoreTest
    {
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

            var sut = new AdminStore<ProtectResource>(session, loggerMock.Object);

            var page = await sut.GetAsync(new PageRequest
            {
                Filter = "Id eq 'test'",
                Skip = 0,
                Take = 10
            });

            Assert.Single(page.Items);

            page = await sut.GetAsync(new PageRequest
            {
                Filter = "DisplayName eq 'no-test'",
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

            var sut = new AdminStore<ProtectResource>(session, loggerMock.Object);

            var page = await sut.GetAsync(new PageRequest
            {
                Expand = "ApiClaims"
            });

            Assert.Single(page.Items);
            Assert.NotNull(page.Items.First().ApiClaims);
            Assert.Single(page.Items.First().ApiClaims);
            Assert.Equal(claimId, page.Items.First().ApiClaims.First().Id);
            Assert.Equal(id, page.Items.First().ApiClaims.First().ApiId);
            Assert.Equal("test", page.Items.First().ApiClaims.First().Type);
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

            var sut = new AdminStore<ApiClaim>(session, loggerMock.Object);

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

            var sut = new AdminStore<ApiClaim>(session, loggerMock.Object);

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
                var sut = new AdminStore<ProtectResource>(session, loggerMock.Object);

                await sut.DeleteAsync(id);
            }


            using var s2 = store.OpenAsyncSession();

            var claimStore = new AdminStore<ApiClaim>(s2, new Mock<ILogger<AdminStore<ApiClaim>>>().Object);
            var claim = await claimStore.GetAsync(claimId, null);
            Assert.Null(claim);
        }
    }
}
