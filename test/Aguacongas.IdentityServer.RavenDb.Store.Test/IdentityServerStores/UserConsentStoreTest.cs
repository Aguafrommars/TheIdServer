// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class UserConsentStoreTest
    {
        [Fact]
        public async Task GetUserConsentAsync_should_return_grant_by_clientId_subjectId()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<UserConsentStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.UserConsent
            {
                Id = "test",
                ClientId = "test",
                UserId = "test",
                Data = serializer.Serialize(new Consent
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.UserConsent)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new UserConsentStore(session, serializer, loggerMock.Object);

            var result = await sut.GetUserConsentAsync("test", "test");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveUserConsentAsync_should_delete_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<UserConsentStore>>();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.UserConsent
            {
                Id = "test",
                ClientId = "test",
                UserId = "test",
                Data = serializer.Serialize(new Consent
                {
                    ClientId = "test"
                })
            }, $"{nameof(Entity.UserConsent)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new UserConsentStore(session, serializer, loggerMock.Object);

            await sut.RemoveUserConsentAsync("test", "test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.UserConsent>($"{nameof(Entity.UserConsent)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveUserConsentAsync_should_not_throw_when_entity_not_exist()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<UserConsentStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new UserConsentStore(session, serializer, loggerMock.Object);

            await sut.RemoveUserConsentAsync("test", "test");

            using var s2 = store.OpenAsyncSession();

            var result = await s2.LoadAsync<Entity.UserConsent>($"{nameof(Entity.UserConsent)}/test");

            Assert.Null(result);
        }

        [Fact]
        public async Task StoreUserConsentAsync_should_create_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<UserConsentStore>>();

            using var session = store.OpenAsyncSession();
            var sut = new UserConsentStore(session, serializer, loggerMock.Object);

            await sut.StoreUserConsentAsync(new Consent
            {
                ClientId = "test",
                SubjectId = "test"
            });

            using var s2 = store.OpenAsyncSession();

            var result = await s2.Advanced.LoadStartingWithAsync<Entity.UserConsent>($"{nameof(Entity.UserConsent).ToLowerInvariant()}/");

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task StoreUserConsentAsync_should_update_entity()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            var serializer = new PersistentGrantSerializer();
            var loggerMock = new Mock<ILogger<UserConsentStore>>();

            var UserConsent = new Consent
            {
                ClientId = "test",
                SubjectId = "test"
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.UserConsent
            {
                Id = "test",
                ClientId = "test",
                UserId = "test",
                Data = serializer.Serialize(UserConsent)
            }, $"{nameof(Entity.UserConsent)}/test");
            await s1.SaveChangesAsync();

            using var session = store.OpenAsyncSession();
            var sut = new UserConsentStore(session, serializer, loggerMock.Object);

            await sut.StoreUserConsentAsync(UserConsent);

            using var s2 = store.OpenAsyncSession();

            var result = await s2.Advanced.LoadStartingWithAsync<Entity.UserConsent>($"{nameof(Entity.UserConsent).ToLowerInvariant()}/");

            Assert.Single(result);
        }
    }
}
