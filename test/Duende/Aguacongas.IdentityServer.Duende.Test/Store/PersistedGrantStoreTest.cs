using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Stores;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.IdentityServer.Duende.Test.Store
{
    public class PersistedGrantStoreTest
    {
        [Fact]
        public void Contructor_should_verify_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(null, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object, new Mock<IAdminStore<AuthorizationCode>>().Object, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, new Mock<IAdminStore<ReferenceToken>>().Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, new Mock<IAdminStore<ReferenceToken>>().Object, new Mock<IAdminStore<RefreshToken>>().Object, null));
        }

        [Fact]
        public async Task GetAllAsync_should_not_be_implemented()
        {
            var sut = new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, 
                new Mock<IAdminStore<ReferenceToken>>().Object, 
                new Mock<IAdminStore<RefreshToken>>().Object, 
                new Mock<IAdminStore<UserConsent>>().Object);
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAllAsync(null));
        }

        [Fact]
        public async Task GetAsync_should_not_be_implemented()
        {
            var sut = new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, 
                new Mock<IAdminStore<ReferenceToken>>().Object, 
                new Mock<IAdminStore<RefreshToken>>().Object, 
                new Mock<IAdminStore<UserConsent>>().Object);
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(null));
        }

        [Fact]
        public async Task RemoveAllAsync_should_create_odata_filter()
        {
            var backChannelAuthenticationRequestStoreMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
            SetupMock(backChannelAuthenticationRequestStoreMock);
            var authorizationCodeStoreMock = new Mock<IAdminStore<AuthorizationCode>>();
            SetupMock(authorizationCodeStoreMock);
            var referenceTokenStoreMock = new Mock<IAdminStore<ReferenceToken>>();
            SetupMock(referenceTokenStoreMock);
            var refreshTokenStoreMock = new Mock<IAdminStore<RefreshToken>>();
            SetupMock(refreshTokenStoreMock);
            var userConsentStoreMock = new Mock<IAdminStore<UserConsent>>();
            SetupMock(userConsentStoreMock);
            var sut = new PersistedGrantStore(backChannelAuthenticationRequestStoreMock.Object,
                authorizationCodeStoreMock.Object, 
                referenceTokenStoreMock.Object, 
                refreshTokenStoreMock.Object, 
                userConsentStoreMock.Object);
            await sut.RemoveAllAsync(new PersistedGrantFilter
            {
                ClientId = "test",
                ClientIds = new[] { "test" },
                SessionId = "test",
                SubjectId = "test",
                Type = PersistedGrantTypes.AuthorizationCode,
                Types = new[] { PersistedGrantTypes.ReferenceToken, PersistedGrantTypes.RefreshToken, PersistedGrantTypes.BackChannelAuthenticationRequest, PersistedGrantTypes.UserConsent }
            });
            authorizationCodeStoreMock.Verify();
        }

        private static void SetupMock<T>(Mock<IAdminStore<T>> mock) where T : class, IEntityId, new()
        {
            mock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PageResponse<T>
            {
                Items = new[] { new T() },
                Count = 1
            }).Verifiable();
            mock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();
        }
    }
}
