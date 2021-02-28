// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class ReferenceTokenStoreTest
    {
        [Fact]
        public async Task GetReferenceTokenAsync_should_return_grant_by_hamlder()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<ReferenceTokenStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ReferenceToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(new Token
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.ReferenceToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new ReferenceTokenStore(session, serializer, loggerMock.Object);

            var result = await sut.GetReferenceTokenAsync("test");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveReferenceTokenAsync_by_handle_should_delete_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<ReferenceTokenStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ReferenceToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(new Token
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.ReferenceToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new ReferenceTokenStore(session, serializer, loggerMock.Object);

            await sut.RemoveReferenceTokenAsync("test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.ReferenceToken>($"{nameof(Entity.ReferenceToken)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveReferenceTokenAsync_by_subjectId_clientId_should_delete_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<ReferenceTokenStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ReferenceToken
            {
                Id = "test",
                ClientId = "test",
                UserId = "test",
                Data = serializer.Serialize(new Token
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.ReferenceToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new ReferenceTokenStore(session, serializer, loggerMock.Object);

            await sut.RemoveReferenceTokensAsync("test", "test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.ReferenceToken>($"{nameof(Entity.ReferenceToken)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveReferenceTokenAsync_should_not_throw_when_entity_not_exist()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<ReferenceTokenStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new ReferenceTokenStore(session, serializer, loggerMock.Object);

            await sut.RemoveReferenceTokenAsync("test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.ReferenceToken>($"{nameof(Entity.ReferenceToken)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task StoreReferenceTokenAsync_should_create_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<ReferenceTokenStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new ReferenceTokenStore(session, serializer, loggerMock.Object);

            var code = await sut.StoreReferenceTokenAsync(new Token
            {
                ClientId = "test"
            });

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.ReferenceToken>($"{nameof(Entity.ReferenceToken)}/{code}");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task StoreReferenceTokenAsync_should_update_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<ReferenceTokenStore>>();

            var token = new Token
            {
                ClientId = "test"
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ReferenceToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(token)
            }, $"{nameof(Entity.ReferenceToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new ReferenceTokenStore(session, serializer, loggerMock.Object);

            var code = await sut.StoreReferenceTokenAsync(token);

            Assert.Equal("test", code);
        }
    }
}
