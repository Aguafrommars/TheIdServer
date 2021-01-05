// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Moq;
using System;
using System.Security.Claims;
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
            var sut = CreateSut();

            await sut.RemoveAllGrantsAsync("test", "test", "test").ConfigureAwait(false);
            
            await sut.RemoveAllGrantsAsync("test", "test").ConfigureAwait(false);

            await sut.RemoveAllGrantsAsync("test").ConfigureAwait(false);

            var grants = await sut.GetAllGrantsAsync("test").ConfigureAwait(false);

            Assert.NotEmpty(grants);
        }

        private static PersistedGrantService CreateSut()
        {
            var authorizationCodeStoreMock = new Mock<IAdminStore<Entity.AuthorizationCode>>();
            var userConsentStoreMock = new Mock<IAdminStore<Entity.UserConsent>>();
            var refreshTokenStoreMock = new Mock<IAdminStore<Entity.RefreshToken>>();
            var referenceTokenStoreMock = new Mock<IAdminStore<Entity.ReferenceToken>>();
            var serializer = new PersistentGrantSerializer();

            var sut = new PersistedGrantService(authorizationCodeStoreMock.Object,
                userConsentStoreMock.Object,
                refreshTokenStoreMock.Object,
                referenceTokenStoreMock.Object,
                serializer);

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
            refreshTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Entity.RefreshToken>
            {
                Items = new[] { new Entity.RefreshToken(){
                    Data = serializer.Serialize(new RefreshToken{
                        AccessToken = new Token
                        {
                            Claims = Array.Empty<Claim>()
                        }
                    })
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
            return sut;
        }
    }
}
