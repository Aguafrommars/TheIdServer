// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class AuthorizationCodeStoreTest
    {
        [Fact]
        public async Task GetAuthorizationCodeAsync_should_return_grant_by_hamlder()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.AuthorizationCode
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(new AuthorizationCode
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.AuthorizationCode)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            var result = await sut.GetAuthorizationCodeAsync("test");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveAuthorizationCodeAsync_should_delete_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.AuthorizationCode
            {
                Id = "test",
                ClientId = "test",
                Data = serializer.Serialize(new AuthorizationCode
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.AuthorizationCode)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            await sut.RemoveAuthorizationCodeAsync("test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.AuthorizationCode>($"{nameof(Entity.AuthorizationCode)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAuthorizationCodeAsync_should_not_throw_when_entity_not_exist()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            await sut.RemoveAuthorizationCodeAsync("test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.AuthorizationCode>($"{nameof(Entity.AuthorizationCode)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task StoreAuthorizationCodeAsync_should_create_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            var code = await sut.StoreAuthorizationCodeAsync(new AuthorizationCode
            {
                ClientId = "test",
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(JwtClaimTypes.Subject, "test") }))
            });

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.AuthorizationCode>($"{nameof(Entity.AuthorizationCode)}/{code}");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task StoreAuthorizationCodeAsync_should_update_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            var authorizationCode = new AuthorizationCode
            {
                ClientId = "test",
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(JwtClaimTypes.Subject, "test") }))
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.AuthorizationCode
            {
                Id = "test",
                ClientId = "test",
                UserId = "test",
                Data = serializer.Serialize(authorizationCode)
            }, $"{nameof(Entity.AuthorizationCode)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            var code = await sut.StoreAuthorizationCodeAsync(authorizationCode);

            Assert.Equal("test", code);
        }

        [Fact]
        public async Task StoreAuthorizationCodeAsync_should_throw_when_subject_is_null()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.StoreAuthorizationCodeAsync(new AuthorizationCode()));
        }

        [Fact]
        public async Task StoreAuthorizationCodeAsync_should_throw_when_subject_claim_is_not_found()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<AuthorizationCodeStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new AuthorizationCodeStore(session, serializer, loggerMock.Object);

            await Assert.ThrowsAsync<Exception>(() => sut.StoreAuthorizationCodeAsync(new AuthorizationCode
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity())
            }));
        }
    }
}
