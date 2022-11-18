// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class PersistedGrantServiceTest
    {
        [Fact]
        public async Task RemoveAllGrantsAsync_should_return_user_grants()
        {
            var sut = CreateSut(out Mock<IDataProtector> _);

            await sut.RemoveAllGrantsAsync("test", "test", "test").ConfigureAwait(false);
            
            await sut.RemoveAllGrantsAsync("test", "test").ConfigureAwait(false);

            await sut.RemoveAllGrantsAsync("test").ConfigureAwait(false);

            var grants = await sut.GetAllGrantsAsync("test").ConfigureAwait(false);

            Assert.NotEmpty(grants);
        }

        [Fact]
        public async Task GetAllGrantsAsync_should_catch_decryption_error()
        {
            var sut = CreateSut(out Mock<IDataProtector> mock);

            mock.Setup(m => m.Unprotect(It.IsAny<byte[]>())).Throws(new CryptographicException());
            var grants = await sut.GetAllGrantsAsync("test").ConfigureAwait(false);

            Assert.NotEmpty(grants);
        }

        private static PersistedGrantService CreateSut(out Mock<IDataProtector> mock)
        {
            var authorizationCodeStoreMock = new Mock<IAdminStore<Entity.AuthorizationCode>>();
            var userConsentStoreMock = new Mock<IAdminStore<Entity.UserConsent>>();
            var refreshTokenStoreMock = new Mock<IAdminStore<Entity.RefreshToken>>();
            var referenceTokenStoreMock = new Mock<IAdminStore<Entity.ReferenceToken>>();
            var dataProtectorProviderMock = new Mock<IDataProtectionProvider>();
            var dataProtectorMock = new Mock<IDataProtector>();
            dataProtectorMock.Setup(m => m.Protect(It.IsAny<byte[]>())).Returns<byte[]>(b => b);
            dataProtectorMock.Setup(m => m.Unprotect(It.IsAny<byte[]>())).Returns<byte[]>(b => b);
            dataProtectorProviderMock.Setup(m => m.CreateProtector(It.IsAny<string>())).Returns(dataProtectorMock.Object);

#if DUENDE
            var serializer = new PersistentGrantSerializer(new PersistentGrantOptions
            {
                DataProtectData = true
            }, dataProtectorProviderMock.Object);
#else
            var serializer = new PersistentGrantSerializer();
#endif
            var localizerMock = new Mock<IStringLocalizer<PersistedGrantService>>();
            var loggerMock = new Mock<ILogger<PersistedGrantService>>();

            var sut = new PersistedGrantService(authorizationCodeStoreMock.Object,
                userConsentStoreMock.Object,
                refreshTokenStoreMock.Object,
                referenceTokenStoreMock.Object,
                serializer,
                localizerMock.Object,
                loggerMock.Object);

            authorizationCodeStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Entity.AuthorizationCode>
            {
                Items = new[] {
                new Entity.AuthorizationCode
                {
                    Data = serializer.Serialize(new AuthorizationCode{
                        RequestedScopes = Array.Empty<string>()
                    })
                }
                }
            });
            userConsentStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Entity.UserConsent>
            {
                Items = new[] { new Entity.UserConsent
                {
                    Data = serializer.Serialize(new Consent{
                        Scopes = Array.Empty<string>()
                    })
                } }
            });
#if DUENDE
            var refreshToken = new RefreshToken();
            refreshToken.SetAccessToken(new Token
            {
                Claims = Array.Empty<Claim>()
            });
#else
            var refreshToken = new RefreshToken
            {
                        AccessToken = new Token
                        {
                            Claims = Array.Empty<Claim>()
                        }
            };
#endif
            refreshTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Entity.RefreshToken>
            {
                Items = new[] { new Entity.RefreshToken(){
                    Data = serializer.Serialize(refreshToken)
                } }
            });
            referenceTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Entity.ReferenceToken>
            {
                Items = new[] { new Entity.ReferenceToken(){
                    Data = serializer.Serialize(new Token{
                        Claims = Array.Empty<Claim>()
                    })
                } }
            });

            mock = dataProtectorMock;
            return sut;
        }
    }
}
