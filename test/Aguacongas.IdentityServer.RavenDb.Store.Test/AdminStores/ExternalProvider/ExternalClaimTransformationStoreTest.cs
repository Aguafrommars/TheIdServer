// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.ExternalProvider
{
    public class LocalizedResourceStoreTest
    {
        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new SchemeDefinition
            {
                Id = "test",                
            }, $"{nameof(SchemeDefinition).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var parent = await s2.LoadAsync<SchemeDefinition>($"{nameof(SchemeDefinition).ToLowerInvariant()}/test");
            var collection = GetCollection(parent);
            Assert.Contains(collection, i => i.Id == $"{typeof(ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new ExternalClaimTransformation
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

            var entity = new ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };
            var parent = new SchemeDefinition
            {
                Id = "test",
            };
            var collection = GetCollection(parent);
            collection.Add(new ExternalClaimTransformation
            {
                Id = $"{typeof(ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}"
            });

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(parent, $"{nameof(SchemeDefinition).ToLowerInvariant()}/{parent.Id}");
            await s1.StoreAsync(entity, $"{typeof(ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var updated = await s2.LoadAsync<SchemeDefinition>($"{nameof(SchemeDefinition).ToLowerInvariant()}/test");
            var updatedCollection = GetCollection(updated);
            Assert.DoesNotContain(updatedCollection, i => i.Id == $"{typeof(ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                Scheme = "test"
            };
            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(entity, $"{typeof(ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<ExternalClaimTransformation>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            Assert.Null(await s2.LoadAsync<ExternalClaimTransformation>($"{typeof(ExternalClaimTransformation).Name.ToLowerInvariant()}/{entity.Id}"));
        }

        private IAdminStore<ExternalClaimTransformation> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ExternalClaimTransformation>> logger)
            => new ExternalClaimTransformationStore(new ScopedAsynDocumentcSession(session), logger);

        private ICollection<ExternalClaimTransformation> GetCollection(SchemeDefinition culture)
        {
            culture.ClaimTransformations = culture.ClaimTransformations ?? new List<ExternalClaimTransformation>();
            return culture.ClaimTransformations;
        }
    }
}
