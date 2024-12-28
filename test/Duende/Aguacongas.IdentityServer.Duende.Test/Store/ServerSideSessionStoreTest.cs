using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Duende.Test.Store
{
    public class ServerSideSessionStoreTest
    {
        [Fact]
        public void Construtot_should_verify_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new ServerSideSessionStore(null));
        }

        [Fact]
        public async Task CreateSessionAsync_should_fallback_to_underliying_store()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            storeMock.Setup(m => m.CreateAsync(It.IsAny<Entity.UserSession>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Entity.UserSession()).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.CreateSessionAsync(new ServerSideSession());

            storeMock.Verify();
        }

        [Fact]
        public async Task DeleteSessionAsync_should_fallback_to_underliying_store()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.DeleteSessionAsync(Guid.NewGuid().ToString());

            storeMock.Verify();
        }

        [Fact]
        public async Task DeleteSessionsAsync_should_create_odata_filter()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            var id = Guid.NewGuid().ToString();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<Entity.UserSession>
            {
                Count = 1,
                Items = new[]
                {
                    new Entity.UserSession
                    {
                        Id = id
                    }
                }
            }).Verifiable();

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.DeleteSessionsAsync(new SessionFilter
            {
                SessionId = id
            });
            
            storeMock.Verify();
        }

        [Fact]
        public async Task GetAndRemoveExpiredSessionsAsync_should_create_odata_filter()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            var id = Guid.NewGuid().ToString();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<Entity.UserSession>
            {
                Count = 1,
                Items = new[]
                {
                    new Entity.UserSession
                    {
                        Id = id,
                        UserId = id
                    }
                }
            }).Verifiable();

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            var result = await sut.GetAndRemoveExpiredSessionsAsync(1);

            storeMock.Verify();

            Assert.Contains(result,  u => u.Key == id);
        }

        [Fact]
        public async Task GetSessionAsync_should_fallback_to_underliying_store()
        {
            var id = Guid.NewGuid().ToString();
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Entity.UserSession
                {
                    Id = id,
                    Renewed = DateTime.UtcNow,
                    Expires = DateTime.UtcNow
                })
                .Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            var session = await sut.GetSessionAsync(id);

            storeMock.Verify();

            Assert.Equal(id, session.Key);
            Assert.Equal(DateTimeKind.Unspecified, session.Renewed.Kind);
            Assert.Equal(DateTimeKind.Unspecified, session.Expires!.Value.Kind);
        }

        [Fact]
        public async Task GetSessionsAsync_should_create_odata_filter()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            var id = Guid.NewGuid().ToString();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<Entity.UserSession>
            {
                Count = 1,
                Items = new[]
                {
                    new Entity.UserSession
                    {
                        Id = id,
                        UserId = id
                    }
                }
            }).Verifiable();


            var sut = new ServerSideSessionStore(storeMock.Object);

            var result = await sut.GetSessionsAsync(new SessionFilter
            {
                SubjectId = id,
            });

            storeMock.Verify();

            Assert.Contains(result, u => u.Key == id);
        }

        [Fact]
        public async Task QuerySessionsAsync_should_create_odata_filter_and_create_page_result()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();

            var pageResponse = new PageResponse<Entity.UserSession>
            {
                Count = 5,
                Items = new[]
                {
                    new Entity.UserSession
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = Guid.NewGuid().ToString()
                    },
                    new Entity.UserSession
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = Guid.NewGuid().ToString()
                    }
                }
            };
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(pageResponse).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            var result = await sut.QuerySessionsAsync();

            storeMock.Verify();

            Assert.Contains(result.Results, u => u.Key == pageResponse.Items.First().Id);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 2,                
            });

            Assert.True(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 2,
                ResultsToken = result.ResultsToken
            });

            Assert.True(result.HasNextResults);
            Assert.True(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(2, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 2,
                ResultsToken = result.ResultsToken
            });
            
            Assert.False(result.HasNextResults);
            Assert.True(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(3, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                RequestPriorResults = true,
                CountRequested = 2,
                ResultsToken = result.ResultsToken
            });
            
            Assert.True(result.HasNextResults);
            Assert.True(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(2, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                RequestPriorResults = true,
                CountRequested = 2,
                ResultsToken = result.ResultsToken
            });
            
            Assert.True(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 1,
                DisplayName = "test"
            });

            Assert.True(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(5, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 5
            });

            Assert.False(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);
        }

        [Fact]
        public async Task UpdateSessionAsync_should_fallback_to_underliying_store()
        {
            var storeMock = new Mock<IAdminStore<Entity.UserSession>>();
            storeMock.Setup(m => m.UpdateAsync(It.IsAny<Entity.UserSession>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.UpdateSessionAsync(new ServerSideSession());

            storeMock.Verify();
        }
    }
}
