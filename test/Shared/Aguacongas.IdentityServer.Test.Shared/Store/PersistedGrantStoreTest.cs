using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
#if DUENDE
using Duende.IdentityServer.Stores;
using static Duende.IdentityServer.IdentityServerConstants;
#else
using IdentityServer4.Stores;
using static IdentityServer4.IdentityServerConstants;
#endif
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Test.Shared.Store
{
    public class PersistedGrantStoreTest
    {
        [Fact]
        public void Contructor_should_verify_parameters()
        {
#if DUENDE
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(null, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object, null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object, new Mock<IAdminStore<AuthorizationCode>>().Object, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, new Mock<IAdminStore<ReferenceToken>>().Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, new Mock<IAdminStore<ReferenceToken>>().Object, new Mock<IAdminStore<RefreshToken>>().Object, null));
#else
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<AuthorizationCode>>().Object, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<AuthorizationCode>>().Object, new Mock<IAdminStore<ReferenceToken>>().Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new PersistedGrantStore(new Mock<IAdminStore<AuthorizationCode>>().Object, new Mock<IAdminStore<ReferenceToken>>().Object, new Mock<IAdminStore<RefreshToken>>().Object, null));
#endif
        }

        [Fact]
        public async Task GetAllAsync_should_not_be_implemented()
        {
#if DUENDE
            var sut = new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, 
                new Mock<IAdminStore<ReferenceToken>>().Object, 
                new Mock<IAdminStore<RefreshToken>>().Object, 
                new Mock<IAdminStore<UserConsent>>().Object);
#else
            var sut = new PersistedGrantStore(new Mock<IAdminStore<AuthorizationCode>>().Object, 
                new Mock<IAdminStore<ReferenceToken>>().Object, 
                new Mock<IAdminStore<RefreshToken>>().Object, 
                new Mock<IAdminStore<UserConsent>>().Object);
#endif
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAllAsync(null));
        }

        [Fact]
        public async Task GetAsync_should_not_be_implemented()
        {
#if DUENDE
            var sut = new PersistedGrantStore(new Mock<IAdminStore<BackChannelAuthenticationRequest>>().Object,
                new Mock<IAdminStore<AuthorizationCode>>().Object, 
                new Mock<IAdminStore<ReferenceToken>>().Object, 
                new Mock<IAdminStore<RefreshToken>>().Object, 
                new Mock<IAdminStore<UserConsent>>().Object);
#else
            var sut = new PersistedGrantStore(new Mock<IAdminStore<AuthorizationCode>>().Object,
                new Mock<IAdminStore<ReferenceToken>>().Object,
                new Mock<IAdminStore<RefreshToken>>().Object,
                new Mock<IAdminStore<UserConsent>>().Object);
#endif
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(null));
        }

        [Fact]
        public async Task RemoveAllAsync_should_create_odata_filter()
        {
#if DUENDE
            var backChannelAuthenticationRequestStoreMock = new Mock<IAdminStore<BackChannelAuthenticationRequest>>();
            SetupMock(backChannelAuthenticationRequestStoreMock);
#endif
            var authorizationCodeStoreMock = new Mock<IAdminStore<AuthorizationCode>>();
            SetupMock(authorizationCodeStoreMock);
            var referenceTokenStoreMock = new Mock<IAdminStore<ReferenceToken>>();
            SetupMock(referenceTokenStoreMock);
            var refreshTokenStoreMock = new Mock<IAdminStore<RefreshToken>>();
            SetupMock(refreshTokenStoreMock);
            var userConsentStoreMock = new Mock<IAdminStore<UserConsent>>();
            SetupMock(userConsentStoreMock);
#if DUENDE
            var sut = new PersistedGrantStore(backChannelAuthenticationRequestStoreMock.Object,
                authorizationCodeStoreMock.Object, 
                referenceTokenStoreMock.Object, 
                refreshTokenStoreMock.Object, 
                userConsentStoreMock.Object);
#else
            var sut = new PersistedGrantStore(authorizationCodeStoreMock.Object,
                referenceTokenStoreMock.Object,
                refreshTokenStoreMock.Object,
                userConsentStoreMock.Object);
#endif
#if DUENDE
            await sut.RemoveAllAsync(new PersistedGrantFilter
            {
                ClientId = "test",
                ClientIds = new[] { "test" },
                SessionId = "test",
                SubjectId = "test",
                Type = PersistedGrantTypes.AuthorizationCode,
                Types = new[] { PersistedGrantTypes.ReferenceToken, PersistedGrantTypes.RefreshToken, PersistedGrantTypes.BackChannelAuthenticationRequest, PersistedGrantTypes.UserConsent }
            });
#else
            await sut.RemoveAllAsync(new PersistedGrantFilter
            {
                ClientId = "test",
                Type = PersistedGrantTypes.AuthorizationCode,
            });
#endif
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
