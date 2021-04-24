// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.User
{
    public abstract class UserSubEntityStoreTestBase<TEntity>
        where TEntity : class, Entity.IEntityId, Entity.IUserSubEntity, new()
    {
        [Fact]
        public async Task CreateAsync_should_add_entity_id_to_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(new Entity.User
            {
                Id = "test"
            }, $"{nameof(Entity.User).ToLowerInvariant()}/test");

            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<TEntity>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new TEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "test"
            };

            await sut.CreateAsync(entity);

            using var s2 = store.OpenAsyncSession();
            var identity = await s2.LoadAsync<Entity.User>($"{nameof(Entity.User).ToLowerInvariant()}/test");
            var collection = GetCollection(identity);
            Assert.Contains(collection, i => i.Id == $"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task CreateAsync_should_throw_when_parent_not_exits()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var loggerMock = new Mock<ILogger<AdminStore<TEntity>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);

            var entity = new TEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "test"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_should_remove_entity_id_from_parent()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new TEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "test"
            };
            var user = new Entity.User
            {
                Id = "test",
            };
            var collection = GetCollection(user);
            collection.Add(new TEntity
            {
                Id = $"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}"
            });

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(user, $"{nameof(Entity.User).ToLowerInvariant()}/{user.Id}");
            await s1.StoreAsync(entity, $"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<TEntity>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            var updated = await s2.LoadAsync<Entity.User>($"{nameof(Entity.User).ToLowerInvariant()}/test");
            var updatedCollection = GetCollection(updated);
            Assert.DoesNotContain(updatedCollection, i => i.Id == $"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}");
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_when_parent_not_exists()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();

            var entity = new TEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "test"
            };

            using var s1 = store.OpenAsyncSession();
            await s1.StoreAsync(entity, $"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}");
            await s1.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<AdminStore<TEntity>>>();

            using var session = store.OpenAsyncSession();

            var sut = CreateSut(session, loggerMock.Object);


            await sut.DeleteAsync(entity.Id);

            using var s2 = store.OpenAsyncSession();
            Assert.Null(await s2.LoadAsync<Entity.User>($"{typeof(TEntity).Name.ToLowerInvariant()}/{entity.Id}"));
        }

        protected abstract IAdminStore<TEntity> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<TEntity>> logger);

        protected abstract ICollection<TEntity> GetCollection(Entity.User user);
    }
}
