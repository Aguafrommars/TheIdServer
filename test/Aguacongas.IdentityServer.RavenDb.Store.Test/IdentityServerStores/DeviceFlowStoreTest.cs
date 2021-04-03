// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class DeviceFlowStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new DeviceFlowStore(new ScopedAsynDocumentcSession(new RavenDbTestDriverWrapper().GetDocumentStore().OpenAsyncSession()), null, new Mock<ILogger<DeviceFlowStore>>().Object));
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_should_find_device_code_by_code()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.DeviceCode
            {
                Id = "test",
                Code = "code",
                Data = serializer.Serialize(new DeviceCode
                {
                    AuthorizedScopes = new[] { "client_credential" }
                })
            }, $"{nameof(Entity.DeviceCode)}/test");
            await s1.SaveChangesAsync();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            var result = await sut.FindByDeviceCodeAsync("code");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_should_throw_when_code_is_null()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.FindByDeviceCodeAsync(null));
        }

        [Fact]
        public async Task FindByUserCodeAsync_should_find_device_code_by_user_code()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.DeviceCode
            {
                Id = "test",
                UserCode = "code",
                Data = serializer.Serialize(new DeviceCode
                {
                    AuthorizedScopes = new[] { "client_credential" }
                })
            }, $"{nameof(Entity.DeviceCode)}/test");
            await s1.SaveChangesAsync();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            var result = await sut.FindByUserCodeAsync("code");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindByUserCodeAsync_should_throw_when_code_is_null()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.FindByUserCodeAsync(null));
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_should_delete_device_code()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.DeviceCode
            {
                Id = "test",
                Code = "code",
                Data = serializer.Serialize(new DeviceCode
                {
                    AuthorizedScopes = new[] { "client_credential" }
                })
            }, $"{nameof(Entity.DeviceCode)}/test");
            await s1.SaveChangesAsync();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await sut.RemoveByDeviceCodeAsync("code");
            await sut.RemoveByDeviceCodeAsync("code");

            using var s2 = store.OpenAsyncSession();

            Assert.Null(await s2.LoadAsync<Entity.DeviceCode>($"{nameof(Entity.DeviceCode)}/test"));
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_should_throw_when_code_is_null()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RemoveByDeviceCodeAsync(null));
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_should_create_device_code()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            var code = Guid.NewGuid().ToString();
            await sut.StoreDeviceAuthorizationAsync(code, "usercode", new DeviceCode
            {
                AuthorizedScopes = new [] {"authorization_code"}
            });

            using var s2 = store.OpenAsyncSession();

            Assert.NotEmpty(await s2.Query<Entity.DeviceCode>().Where(d => d.Code == code).ToListAsync());
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_should_check_parameters()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.StoreDeviceAuthorizationAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.StoreDeviceAuthorizationAsync("code", null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.StoreDeviceAuthorizationAsync("code", "userCode", null));
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_update_device_code()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.DeviceCode
            {
                Id = "test",
                UserCode = "code",
                Data = serializer.Serialize(new DeviceCode
                {
                    AuthorizedScopes = new[] { "client_credential" }
                })
            }, $"{nameof(Entity.DeviceCode)}/test");
            await s1.SaveChangesAsync();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await sut.UpdateByUserCodeAsync("code", new DeviceCode
            {
                ClientId = "test"
            });

            using var s2 = store.OpenAsyncSession();

            var updated = await s2.LoadAsync<Entity.DeviceCode>($"{nameof(Entity.DeviceCode)}/test");
            var data = serializer.Deserialize<DeviceCode>(updated.Data);

            Assert.Equal("test", data.ClientId);
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_throw_when_code_not_found()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateByUserCodeAsync("code", new DeviceCode
            {
                ClientId = "test"
            }));
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_check_parameters()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var sut = new DeviceFlowStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UpdateByUserCodeAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.UpdateByUserCodeAsync("code", null));
        }
    }
}
