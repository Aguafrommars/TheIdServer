// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class UserConsentStoreTest
    {
        [Fact]
        public async Task GetUserConsentAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<UserConsent>> storeMock,
                out UserConsentStore sut);

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<UserConsent>
                {
                    Count = 1,
                    Items = new List<UserConsent>
                    {
                        new UserConsent
                        {
                            Id = "id"
                        }
                    }
                })
                .Verifiable();

            await sut.GetUserConsentAsync("test", "test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq 'test' And ClientId eq 'test'"), default));
        }

        [Fact]
        public async Task RemoveUserConsentAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<UserConsent>> storeMock,
                out UserConsentStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<UserConsent>
                {
                    Count = 1,
                    Items = new List<UserConsent>
                    {
                        new UserConsent
                        {
                            Id = "id"
                        }
                    }
                })
                .Verifiable();

            await sut.RemoveUserConsentAsync("test", "test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq 'test' And ClientId eq 'test'"), default));
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task StoreUserConsentAsync_should_call_store_CreateAsync()
        {
            CreateSut(out Mock<IAdminStore<UserConsent>> storeMock,
                out UserConsentStore sut);

            storeMock.Setup(m => m.CreateAsync(It.IsAny<UserConsent>(), default))
                .ReturnsAsync(new UserConsent())
                .Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<UserConsent>
                {
                    Count = 0,
                    Items = new List<UserConsent>(0)
                })
                .Verifiable();

            await sut.StoreUserConsentAsync(new IdentityServer4.Models.Consent
            {
                ClientId = "test",
                SubjectId = "test"
            });

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq 'test' And ClientId eq 'test'"), default));
            storeMock.Verify(m => m.CreateAsync(It.IsAny<UserConsent>(), default));
        }

        private static void CreateSut(out Mock<IAdminStore<UserConsent>> storeMock,
            out UserConsentStore sut)
        {
            storeMock = new Mock<IAdminStore<UserConsent>>();
            var serializerMock = new Mock<IPersistentGrantSerializer>();
            sut = new UserConsentStore(storeMock.Object, serializerMock.Object);
        }
    }
}
