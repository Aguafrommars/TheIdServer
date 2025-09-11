// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores
{
    public class LocalizedResourceStoreTest
    {
        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Culture
            {
                Id = "test"
            }, $"{nameof(Culture).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<LocalizedResource>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new LocalizedResource
            {
                Id = Guid.NewGuid().ToString(),
                CultureId = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var parent = await s2.LoadAsync<Culture>($"{nameof(Culture).ToLowerInvariant()}/test");
            var collection = GetCollection(parent);
            Assert.Contains(collection, i => i.Id == $"{typeof(LocalizedResource).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<LocalizedResource>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new LocalizedResource
            {
                Id = Guid.NewGuid().ToString(),
                CultureId = "test"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_should_remove_entity_id_from_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new LocalizedResource
            {
                Id = Guid.NewGuid().ToString(),
                CultureId = "test"
            };
            var parent = new Culture
            {
                Id = "test",
            };
            var collection = GetCollection(parent);
            collection.Add(new LocalizedResource
            {
                Id = $"{typeof(LocalizedResource).Name.ToLowerInvariant()}/{entity.Id}"
            });

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(parent, $"{nameof(Culture).ToLowerInvariant()}/{parent.Id}");
            await s1.StoreAsync(entity, $"{typeof(LocalizedResource).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<LocalizedResource>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var updated = await s2.LoadAsync<Culture>($"{nameof(Culture).ToLowerInvariant()}/test");
            var updatedCollection = GetCollection(updated);
            Assert.DoesNotContain(updatedCollection, i => i.Id == $"{typeof(LocalizedResource).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new LocalizedResource
            {
                Id = Guid.NewGuid().ToString(),
                CultureId = "test"
            };
            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(entity, $"{typeof(LocalizedResource).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<LocalizedResource>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            Assert.Null(await s2.LoadAsync<LocalizedResource>($"{typeof(LocalizedResource).Name.ToLowerInvariant()}/{entity.Id}"));
        }

        private static IAdminStore<LocalizedResource> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<LocalizedResource>> logger)
            => new LocalizedResourceStore(new ScopedAsynDocumentcSession(session), logger);

        private static ICollection<LocalizedResource> GetCollection(Culture culture)
        {
            culture.Resources = culture.Resources ?? new List<LocalizedResource>();
            return culture.Resources;
        }
    }
}
