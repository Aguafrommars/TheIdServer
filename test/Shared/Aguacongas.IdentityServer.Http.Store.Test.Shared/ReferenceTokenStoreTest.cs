// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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
    public class ReferenceTokenStoreTest
    {
        [Fact]
        public async Task GetReferenceTokenAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ReferenceToken>> storeMock,
                out ReferenceTokenStore sut);

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new ReferenceToken())
                .Verifiable();

            await sut.GetReferenceTokenAsync("test");

            storeMock.Verify(m => m.GetAsync("test", null, default));
        }

        [Fact]
        public async Task RemoveReferenceTokenAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<ReferenceToken>> storeMock,
                out ReferenceTokenStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new ReferenceToken
                {
                    Id = "id"
                })
                .Verifiable();

            await sut.RemoveReferenceTokenAsync("test");

            storeMock.Verify(m => m.GetAsync("test", null, default));
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task RemoveReferenceTokensAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<ReferenceToken>> storeMock,
                out ReferenceTokenStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ReferenceToken>
                {
                    Count = 1,
                    Items = new List<ReferenceToken>
                    {
                        new ReferenceToken
                        {
                            Id = "id"
                        }
                    }
                })
                .Verifiable();

            await sut.RemoveReferenceTokensAsync("test", "test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq 'test' and ClientId eq 'test'"), default));
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task StoreReferenceTokenAsync_should_call_store_CreateAsync()
        {
            CreateSut(out Mock<IAdminStore<ReferenceToken>> storeMock,
                out ReferenceTokenStore sut);

            storeMock.Setup(m => m.CreateAsync(It.IsAny<ReferenceToken>(), default))
                .ReturnsAsync(new ReferenceToken())
                .Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ReferenceToken>
                {
                    Count = 0,
                    Items = new List<ReferenceToken>(0)
                })
                .Verifiable();

            await sut.StoreReferenceTokenAsync(new ISModels.Token()
            {
                ClientId = "test"
            });

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq '' and ClientId eq 'test'"), default));
            storeMock.Verify(m => m.CreateAsync(It.IsAny<ReferenceToken>(), default));

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ReferenceToken>
                {
                    Count = 1,
                    Items = new List<ReferenceToken>
                    {
                        new ReferenceToken()
                    }
                })
                .Verifiable();
            storeMock.Setup(m => m.UpdateAsync(It.IsAny<ReferenceToken>(), default))
                .ReturnsAsync(new ReferenceToken())
                .Verifiable();

            await sut.StoreReferenceTokenAsync(new ISModels.Token()
            {
                ClientId = "test"
            });

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq '' and ClientId eq 'test'"), default));
            storeMock.Verify(m => m.UpdateAsync(It.IsAny<ReferenceToken>(), default));
        }

        private static void CreateSut(out Mock<IAdminStore<ReferenceToken>> storeMock,
            out ReferenceTokenStore sut)
        {
            storeMock = new Mock<IAdminStore<ReferenceToken>>();
            var serializerMock = new Mock<IPersistentGrantSerializer>();
            sut = new ReferenceTokenStore(storeMock.Object, serializerMock.Object);
        }
    }
}
