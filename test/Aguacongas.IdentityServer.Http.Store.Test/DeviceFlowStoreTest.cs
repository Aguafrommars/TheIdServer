// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class DeviceFlowStoreTest
    {
        [Fact]
        public async Task FindByDeviceCodeAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<DeviceCode>> storeMock,
                out DeviceFlowStore sut);

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>())
                .Verifiable();

            await sut.FindByDeviceCodeAsync("test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(r => r.Filter == "Code eq 'test'"), default));

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>
                {
                    Count = 1,
                    Items = new List<DeviceCode>
                    {
                        new DeviceCode()
                    }
                })
                .Verifiable();

            await sut.FindByDeviceCodeAsync("test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(r => r.Filter == "Code eq 'test'"), default));
        }

        [Fact]
        public async Task FindByUserCodeAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<DeviceCode>> storeMock,
                out DeviceFlowStore sut);

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>())
                .Verifiable();

            await sut.FindByUserCodeAsync("test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(r => r.Filter == "UserCode eq 'test'"), default));

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>
                {
                    Count = 1,
                    Items = new List<DeviceCode>
                    {
                        new DeviceCode()
                    }
                })
                .Verifiable();

            await sut.FindByUserCodeAsync("test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(r => r.Filter == "UserCode eq 'test'"), default));
        }

        [Fact]
        public async Task RemoveDeviceCodeAsync_should_call_store_DeleteAsync()
        {
            CreateSut(out Mock<IAdminStore<DeviceCode>> storeMock,
                out DeviceFlowStore sut);

            storeMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), default)).Verifiable();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>
                {
                    Count = 1,
                    Items = new List<DeviceCode>
                    {
                        new DeviceCode
                        {
                            Id = "id"
                        }
                    }
                })
                .Verifiable();

            await sut.RemoveByDeviceCodeAsync("test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(r => r.Filter == "Code eq 'test'"), default));
            storeMock.Verify(m => m.DeleteAsync(It.Is<string>(r => r == "id"), default));
        }

        [Fact]
        public async Task StoreDeviceCodeAsync_should_call_store_CreateAsync()
        {
            CreateSut(out Mock<IAdminStore<DeviceCode>> storeMock,
                out DeviceFlowStore sut);

            storeMock.Setup(m => m.CreateAsync(It.IsAny<DeviceCode>(), default))
                .ReturnsAsync(new DeviceCode())
                .Verifiable();

            await sut.StoreDeviceAuthorizationAsync("test", "test", new IdentityServer4.Models.DeviceCode());

            storeMock.Verify(m => m.CreateAsync(It.IsAny<DeviceCode>(), default));
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_call_store_CreateAsync()
        {
            CreateSut(out Mock<IAdminStore<DeviceCode>> storeMock,
                out DeviceFlowStore sut);

            storeMock.Setup(m => m.UpdateAsync(It.IsAny<DeviceCode>(), default))
                .ReturnsAsync(new DeviceCode())
                .Verifiable();

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>
                {
                    Count = 1,
                    Items = new List<DeviceCode>
                    {
                        new DeviceCode
                        {
                            Id = "id"
                        }
                    }
                })
                .Verifiable();

            await sut.UpdateByUserCodeAsync("test", new IdentityServer4.Models.DeviceCode());

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(r => r.Filter == "UserCode eq 'test'"), default));
            storeMock.Verify(m => m.UpdateAsync(It.IsAny<DeviceCode>(), default));

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<DeviceCode>
                {
                    Count = 0,
                })
                .Verifiable();

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateByUserCodeAsync("test", new IdentityServer4.Models.DeviceCode()));
        }

        private static void CreateSut(out Mock<IAdminStore<DeviceCode>> storeMock,
            out DeviceFlowStore sut)
        {
            storeMock = new Mock<IAdminStore<DeviceCode>>();
            var serializerMock = new Mock<IPersistentGrantSerializer>();
            sut = new DeviceFlowStore(storeMock.Object, serializerMock.Object);
        }
    }
}
