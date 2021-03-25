// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityAdminStores
{
    public class IdentityRoleStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleStore<IdentityUser, IdentityRole>(null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            Assert.Throws<ArgumentNullException>(() => new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, null, null));
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), null));
        }

        [Fact]
        public async Task CreateAsync_should_return_role_id()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());
            var result = await sut.CreateAsync(new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            } as object);


            Assert.NotNull(result);
            Assert.NotNull(((Role)result).Id);
        }

        [Fact]
        public async Task DeleteAsync_should_delete_role()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var result = await roleManager.CreateAsync(role);
            Assert.True(result.Succeeded);

            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());
            await sut.DeleteAsync(role.Id);

            var actual = await roleManager.FindByIdAsync(role.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async Task UdpateAsync_should_update_role()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var result = await roleManager.CreateAsync(role);
            Assert.True(result.Succeeded);

            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());
            await sut.UpdateAsync(new Role
            {
                Id = role.Id,
                Name = "test"
            } as object);

            var actual = await roleManager.FindByIdAsync(role.Id);
            Assert.NotNull(actual);
            Assert.Equal("test", actual.Name);
        }


        [Fact]
        public async Task GetAsync_by_page_request_should_find_roles()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);
            

            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());

            var rolesResult = await sut.GetAsync(new PageRequest
            {
                Filter = $"Name eq '{role.Name}'"
            });

            Assert.NotNull(rolesResult);
            Assert.Equal(1, rolesResult.Count);
            Assert.Single(rolesResult.Items);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_role()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);
            

            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());

            var result = await sut.GetAsync(role.Id, null);

            Assert.NotNull(result);
        }
    }
}
