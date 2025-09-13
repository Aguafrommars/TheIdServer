// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.AdminStores.Role;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Role
{
    public class RoleClaimStoreTest
    {
        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.Role
            {
                Id = "test",
            }, $"{nameof(Entity.Role).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RoleClaim>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new Entity.RoleClaim
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var parent = await s2.LoadAsync<Entity.Role>($"{nameof(Entity.Role).ToLowerInvariant()}/test");
            var collection = GetCollection(parent);
            Assert.Contains(collection, i => i.Id == $"{typeof(Entity.RoleClaim).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RoleClaim>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new Entity.RoleClaim
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = "test"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_should_remove_entity_id_from_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.RoleClaim
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = "test"
            };
            var parent = new Entity.Role
            {
                Id = "test",
            };
            var collection = GetCollection(parent);
            collection.Add(new Entity.RoleClaim
            {
                Id = $"{typeof(Entity.RoleClaim).Name.ToLowerInvariant()}/{entity.Id}"
            });

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(parent, $"{nameof(Entity.Role).ToLowerInvariant()}/{parent.Id}");
            await s1.StoreAsync(entity, $"{typeof(Entity.RoleClaim).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RoleClaim>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var updated = await s2.LoadAsync<Entity.Role>($"{nameof(Entity.Role).ToLowerInvariant()}/test");
            var updatedCollection = GetCollection(updated);
            Assert.DoesNotContain(updatedCollection, i => i.Id == $"{typeof(Entity.RoleClaim).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new Entity.RoleClaim
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = "test"
            };
            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(entity, $"{typeof(Entity.RoleClaim).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<Entity.RoleClaim>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            Assert.Null(await s2.LoadAsync<Entity.RoleClaim>($"{typeof(Entity.RoleClaim).Name.ToLowerInvariant()}/{entity.Id}"));
        }

        private static IAdminStore<Entity.RoleClaim> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<Entity.RoleClaim>> logger)
            => new RoleClaimStore(new ScopedAsynDocumentcSession(session), logger);

        private static ICollection<Entity.RoleClaim> GetCollection(Entity.Role role)
        {
            role.RoleClaims = role.RoleClaims ?? new List<Entity.RoleClaim>();
            return role.RoleClaims;
        }
    }
}
