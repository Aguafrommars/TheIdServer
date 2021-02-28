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
    public class RefreshTokenStoreTest
    {
        [Fact]
        public async Task GetRefreshTokenAsync_should_return_grant_by_hamlder()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.RefreshToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(new Token
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.RefreshToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new RefreshTokenStore(session, serializer, loggerMock.Object);

            var result = await sut.GetRefreshTokenAsync("test");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveRefreshTokenAsync_by_handle_should_delete_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.RefreshToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(new Token
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.RefreshToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new RefreshTokenStore(session, serializer, loggerMock.Object);

            await sut.RemoveRefreshTokenAsync("test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.RefreshToken>($"{nameof(Entity.RefreshToken)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveRefreshTokenAsync_by_subjectId_clientId_should_delete_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.RefreshToken
            {
                Id = "test",
                ClientId = "test",
                UserId = "test",
                Data = serializer.Serialize(new Token
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.RefreshToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new RefreshTokenStore(session, serializer, loggerMock.Object);

            await sut.RemoveRefreshTokensAsync("test", "test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.RefreshToken>($"{nameof(Entity.RefreshToken)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveRefreshTokenAsync_should_not_throw_when_entity_not_exist()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new RefreshTokenStore(session, serializer, loggerMock.Object);

            await sut.RemoveRefreshTokenAsync("test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.RefreshToken>($"{nameof(Entity.RefreshToken)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task StoreRefreshTokenAsync_should_create_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new RefreshTokenStore(session, serializer, loggerMock.Object);

            var code = await sut.StoreRefreshTokenAsync(new RefreshToken
            {
                AccessToken = new Token
                {
                    ClientId = "test"
                }
            });

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.RefreshToken>($"{nameof(Entity.RefreshToken)}/{code}");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task StoreRefreshTokenAsync_should_update_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            var token = new RefreshToken
            {
                AccessToken = new Token
                {
                    ClientId = "test"
                }
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.RefreshToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(token)
            }, $"{nameof(Entity.RefreshToken)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new RefreshTokenStore(session, serializer, loggerMock.Object);

            var code = await sut.StoreRefreshTokenAsync(token);

            Assert.Equal("test", code);
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_update_token()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            var token = new RefreshToken
            {
                AccessToken = new Token
                {
                    ClientId = "test"
                }
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.RefreshToken
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(token)
            }, $"{nameof(Entity.RefreshToken)}/test");
            await s1.SaveChangesAsync();

            var sut = new RefreshTokenStore(store.OpenAsyncSession(), serializer, loggerMock.Object);

            await sut.UpdateRefreshTokenAsync("test", new RefreshToken
            {
                AccessToken = new Token
                {
                    ClientId = "test",
                    Description = "test"
                }
            });

            using var s2 = store.OpenAsyncSession();

            var updated = await s2.LoadAsync<Entity.RefreshToken>($"{nameof(Entity.RefreshToken)}/test");
            var data = serializer.Deserialize<RefreshToken>(updated.Data);

            Assert.Equal("test", data.AccessToken.Description);
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_throw_when_handle_not_found()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<RefreshTokenStore>>();

            var sut = new RefreshTokenStore(store.OpenAsyncSession(), serializer, loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateRefreshTokenAsync("handle", new RefreshToken()));
        }
    }
}