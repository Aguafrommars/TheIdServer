// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.ExternalProvider
{
    public class ExternalClaimTransformationStoreTest
    {
        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.ExternalProvider
            {
                Id = "test",                
            }, $"{nameof(Entity.ExternalProvider).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new Entity.ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var parent = await s2.LoadAsync<Entity.ExternalProvider>($"{nameof(Entity.ExternalProvider).ToLowerInvariant()}/test");
            var collection = GetCollection(parent);
            Assert.Contains(collection, i => i.Id == $"{typeof(Entity.ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new Entity.ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_should_remove_entity_id_from_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };
            var parent = new Entity.ExternalProvider
            {
                Id = "test",
            };
            var collection = GetCollection(parent);
            collection.Add(new Entity.ExternalClaimTransformation
            {
                Id = $"{typeof(Entity.ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}"
            });

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(parent, $"{nameof(Entity.ExternalProvider).ToLowerInvariant()}/{parent.Id}");
            await s1.StoreAsync(entity, $"{typeof(Entity.ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var updated = await s2.LoadAsync<Entity.ExternalProvider> ($"{nameof(Entity.ExternalProvider).ToLowerInvariant()}/test");
            var updatedCollection = GetCollection(updated);
            Assert.DoesNotContain(updatedCollection, i => i.Id == $"{typeof(Entity.ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };
            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(entity, $"{typeof(Entity.ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            Assert.Null(await s2.LoadAsync<Entity.ExternalClaimTransformation>($"{typeof(Entity.ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}"));
        }

        private static IAdminStore<Entity.ExternalClaimTransformation> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ExternalClaimTransformation>> logger)
            => new ExternalClaimTransformationStore(new ScopedAsynDocumentcSession(session), logger);

        private static ICollection<Entity.ExternalClaimTransformation> GetCollection(Entity.ExternalProvider provider)
        {
            provider.ClaimTransformations = provider.ClaimTransformations ?? new List<Entity.ExternalClaimTransformation>();
            return provider.ClaimTransformations;
        }
    }
}
