// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.RelyingParty;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.RelyingParty
{
    public class RelyingPartyClaimMappingStoreTest
    {
        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.RelyingParty
            {
                Id = "test",                
            }, $"{nameof(Entity.RelyingParty).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RelyingPartyClaimMapping>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new Entity.RelyingPartyClaimMapping
            {
                Id = Guid.NewGuid().ToString(),
                RelyingPartyId = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var parent = await s2.LoadAsync<Entity.RelyingParty>($"{nameof(Entity.RelyingParty).ToLowerInvariant()}/test");
            var collection = GetCollection(parent);
            Assert.Contains(collection, i => i.Id == $"{typeof(Entity.RelyingPartyClaimMapping).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RelyingPartyClaimMapping>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new Entity.RelyingPartyClaimMapping
            {
                Id = Guid.NewGuid().ToString(),
                RelyingPartyId = "test"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_should_remove_entity_id_from_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.RelyingPartyClaimMapping
            {
                Id = Guid.NewGuid().ToString(),
                RelyingPartyId = "test"
            };
            var parent = new Entity.RelyingParty
            {
                Id = "test",
            };
            var collection = GetCollection(parent);
            collection.Add(new Entity.RelyingPartyClaimMapping
            {
                Id = $"{typeof(Entity.RelyingPartyClaimMapping).Name.ToLowerInvariant()}/{entity.Id}"
            });

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(parent, $"{nameof(Entity.RelyingParty).ToLowerInvariant()}/{parent.Id}");
            await s1.StoreAsync(entity, $"{typeof(Entity.RelyingPartyClaimMapping).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RelyingPartyClaimMapping>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var updated = await s2.LoadAsync<Entity.RelyingParty> ($"{nameof(Entity.RelyingParty).ToLowerInvariant()}/test");
            var updatedCollection = GetCollection(updated);
            Assert.DoesNotContain(updatedCollection, i => i.Id == $"{typeof(Entity.RelyingPartyClaimMapping).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.RelyingPartyClaimMapping
            {
                Id = Guid.NewGuid().ToString(),
                RelyingPartyId = "test"
            };
            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(entity, $"{typeof(Entity.RelyingPartyClaimMapping).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RelyingPartyClaimMapping>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            Assert.Null(await s2.LoadAsync<Entity.RelyingPartyClaimMapping>($"{typeof(Entity.RelyingPartyClaimMapping).Name.ToLowerInvariant()}/{entity.Id}"));
        }

        private IAdminStore<Entity.RelyingPartyClaimMapping> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<Entity.RelyingPartyClaimMapping>> logger)
            => new RelyingPartyClaimMappingStore(new ScopedAsynDocumentcSession(session), logger);

        private ICollection<Entity.RelyingPartyClaimMapping> GetCollection(Entity.RelyingParty provider)
        {
            provider.ClaimMappings ??= new List<Entity.RelyingPartyClaimMapping>();
            return provider.ClaimMappings;
        }
    }
}
