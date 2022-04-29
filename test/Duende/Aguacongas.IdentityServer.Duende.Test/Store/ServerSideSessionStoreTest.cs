using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
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
            var storeMock = new Mock<IAdminStore<UserSession>>();
            storeMock.Setup(m => m.CreateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>())).ReturnsAsync(new UserSession()).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.CreateSessionAsync(new ServerSideSession()).ConfigureAwait(false);

            storeMock.Verify();
        }

        [Fact]
        public async Task DeleteSessionAsync_should_fallback_to_underliying_store()
        {
            var storeMock = new Mock<IAdminStore<UserSession>>();
            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.DeleteSessionAsync(Guid.NewGuid().ToString()).ConfigureAwait(false);

            storeMock.Verify();
        }

        [Fact]
        public async Task DeleteSessionsAsync_should_create_odata_filter()
        {
            var storeMock = new Mock<IAdminStore<UserSession>>();
            var id = Guid.NewGuid().ToString();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<UserSession>
            {
                Count = 1,
                Items = new[]
                {
                    new UserSession
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
            }).ConfigureAwait(false);
            
            storeMock.Verify();
        }

        [Fact]
        public async Task GetAndRemoveExpiredSessionsAsync_should_create_odata_filter()
        {
            var storeMock = new Mock<IAdminStore<UserSession>>();
            var id = Guid.NewGuid().ToString();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<UserSession>
            {
                Count = 1,
                Items = new[]
                {
                    new UserSession
                    {
                        Id = id,
                        UserId = id
                    }
                }
            }).Verifiable();

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            var result = await sut.GetAndRemoveExpiredSessionsAsync(1).ConfigureAwait(false);

            storeMock.Verify();

            Assert.Contains(result,  u => u.Key == id);
        }

        [Fact]
        public async Task GetSessionAsync_should_fallback_to_underliying_store()
        {
            var id = Guid.NewGuid().ToString();
            var storeMock = new Mock<IAdminStore<UserSession>>();
            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserSession
                {
                    Id = id
                })
                .Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            var session = await sut.GetSessionAsync(id).ConfigureAwait(false);

            storeMock.Verify();

            Assert.Equal(id, session.Key);
        }

        [Fact]
        public async Task GetSessionsAsync_should_create_odata_filter()
        {
            var storeMock = new Mock<IAdminStore<UserSession>>();
            var id = Guid.NewGuid().ToString();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<UserSession>
            {
                Count = 1,
                Items = new[]
                {
                    new UserSession
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
            }).ConfigureAwait(false);

            storeMock.Verify();

            Assert.Contains(result, u => u.Key == id);
        }

        [Fact]
        public async Task QuerySessionsAsync_should_create_odata_filter_and_create_page_result()
        {
            var storeMock = new Mock<IAdminStore<UserSession>>();

            var pageResponse = new PageResponse<UserSession>
            {
                Count = 5,
                Items = new[]
                {
                    new UserSession
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = Guid.NewGuid().ToString()
                    },
                    new UserSession
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = Guid.NewGuid().ToString()
                    }
                }
            };
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(pageResponse).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            var result = await sut.QuerySessionsAsync().ConfigureAwait(false);

            storeMock.Verify();

            Assert.Contains(result.Results, u => u.Key == pageResponse.Items.First().Id);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 2,                
            }).ConfigureAwait(false);

            Assert.True(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 2,
                ResultsToken = result.ResultsToken
            }).ConfigureAwait(false);

            Assert.True(result.HasNextResults);
            Assert.True(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(2, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 2,
                ResultsToken = result.ResultsToken
            }).ConfigureAwait(false);
            
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
            }).ConfigureAwait(false);
            
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
            }).ConfigureAwait(false);
            
            Assert.True(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 1,
                DisplayName = "test"
            }).ConfigureAwait(false);

            Assert.True(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(5, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);

            result = await sut.QuerySessionsAsync(new SessionQuery
            {
                CountRequested = 5
            }).ConfigureAwait(false);

            Assert.False(result.HasNextResults);
            Assert.False(result.HasPrevResults);
            Assert.Equal(pageResponse.Count, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(1, result.CurrentPage);
        }

        [Fact]
        public async Task UpdateSessionAsync_should_fallback_to_underliying_store()
        {
            var storeMock = new Mock<IAdminStore<UserSession>>();
            storeMock.Setup(m => m.UpdateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>())).Verifiable();

            var sut = new ServerSideSessionStore(storeMock.Object);

            await sut.UpdateSessionAsync(new ServerSideSession()).ConfigureAwait(false);

            storeMock.Verify();
        }
    }
}
