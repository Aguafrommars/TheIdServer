// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
#if DUENDE
using Duende.IdentityServer.Stores.Serialization;
using ISModels = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores.Serialization;
using ISModels = IdentityServer4.Models;
#endif
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class RefreshTokenStoreTest
    {
        [Fact]
        public async Task GetRefreshTokenAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<RefreshToken>> storeMock,
                out RefreshTokenStore sut);

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new RefreshToken())
                .Verifiable();

            await sut.GetRefreshTokenAsync("test");

            storeMock.Verify(m => m.GetAsync("test", null, default));
        }

        [Fact]
        public async Task RemoveRefreshTokenAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<RefreshToken>> storeMock,
                out RefreshTokenStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new RefreshToken
                {
                    Id = "id"
                })
                .Verifiable();

            await sut.RemoveRefreshTokenAsync("test");

            storeMock.Verify(m => m.GetAsync("test", null, default));
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task RemoveRefreshTokensAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<RefreshToken>> storeMock,
                out RefreshTokenStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<RefreshToken>
                {
                    Count = 1,
                    Items = new List<RefreshToken>
                    {
                        new RefreshToken
                        {
                            Id = "id"
                        }
                    }
                })
                .Verifiable();

            await sut.RemoveRefreshTokensAsync("test", "test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq 'test' and ClientId eq 'test'"), default));
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task StoreRefreshTokenAsync_should_call_store_CreateAsync()
        {
            CreateSut(out Mock<IAdminStore<RefreshToken>> storeMock,
                out RefreshTokenStore sut);

            storeMock.Setup(m => m.CreateAsync(It.IsAny<RefreshToken>(), default))
                .ReturnsAsync(new RefreshToken())
                .Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<RefreshToken>
                {
                    Count = 0,
                    Items = new List<RefreshToken>(0)
                })
                .Verifiable();

            await sut.StoreRefreshTokenAsync(new ISModels.RefreshToken
            {
                AccessToken = new ISModels.Token
                {
                    ClientId = "test"
                }
            });

            storeMock.Verify(m => m.GetAsync(It.IsAny<PageRequest>(), default));
            storeMock.Verify(m => m.CreateAsync(It.IsAny<RefreshToken>(), default));
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_should_call_store_UpdateAsync()
        {
            CreateSut(out Mock<IAdminStore<RefreshToken>> storeMock,
                out RefreshTokenStore sut);

            storeMock.Setup(m => m.UpdateAsync(It.IsAny<RefreshToken>(), default))
                .ReturnsAsync(new RefreshToken())
                .Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new RefreshToken())
                .Verifiable();

            await sut.UpdateRefreshTokenAsync("test", new ISModels.RefreshToken
            {
                AccessToken = new ISModels.Token
                {
                    ClientId = "test"
                }
            });

            storeMock.Verify(m => m.GetAsync("test", null, default));
            storeMock.Verify(m => m.UpdateAsync(It.IsAny<RefreshToken>(), default));
        }

        private static void CreateSut(out Mock<IAdminStore<RefreshToken>> storeMock,
            out RefreshTokenStore sut)
        {
            storeMock = new Mock<IAdminStore<RefreshToken>>();
            var serializerMock = new Mock<IPersistentGrantSerializer>();
            sut = new RefreshTokenStore(storeMock.Object, serializerMock.Object);
        }
    }
}
