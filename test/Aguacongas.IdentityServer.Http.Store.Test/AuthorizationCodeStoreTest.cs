﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class AuthorizationCodeStoreTest
    {
        [Fact]
        public async Task GetAuthorizationCodeAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<AuthorizationCode>> storeMock,
                out AuthorizationCodeStore sut);

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), null, default))
                .ReturnsAsync(new AuthorizationCode())
                .Verifiable();

            await sut.GetAuthorizationCodeAsync("test");

            storeMock.Verify(m => m.GetAsync("test", null, default));
        }

        [Fact]
        public async Task RemoveAuthorizationCodeAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<AuthorizationCode>> storeMock,
                out AuthorizationCodeStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();
            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new AuthorizationCode
                        {
                            Id = "id"
                        })
                .Verifiable();

            await sut.RemoveAuthorizationCodeAsync("test");

            storeMock.Verify(m => m.GetAsync("test", null, default)); 
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task StoreAuthorizationCodeAsync_should_call_store_CreateAsync()
        {
            CreateSut(out Mock<IAdminStore<AuthorizationCode>> storeMock,
                out AuthorizationCodeStore sut);

            storeMock.Setup(m => m.CreateAsync(It.IsAny<AuthorizationCode>(), default))
                .ReturnsAsync(new AuthorizationCode())
                .Verifiable();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<AuthorizationCode>
                {
                    Count = 0,
                    Items = new List<AuthorizationCode>(0)
                })
                .Verifiable();
            await sut.StoreAuthorizationCodeAsync(new IdentityServer4.Models.AuthorizationCode
            {
                ClientId = "test",
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new [] { new Claim(ClaimTypes.NameIdentifier, "test") }))
            });

            storeMock.Verify(m => m.CreateAsync(It.IsAny<AuthorizationCode>(), default));
        }

        private static void CreateSut(out Mock<IAdminStore<AuthorizationCode>> storeMock,
            out AuthorizationCodeStore sut)
        {
            storeMock = new Mock<IAdminStore<AuthorizationCode>>();
            var serializerMock = new Mock<IPersistentGrantSerializer>();
            sut = new AuthorizationCodeStore(storeMock.Object, serializerMock.Object);
        }
    }
}
