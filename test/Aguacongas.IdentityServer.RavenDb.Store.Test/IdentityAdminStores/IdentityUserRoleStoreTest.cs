// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;


namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityAdminStores
{
    public class IdentityUserRoleStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserRoleStore<IdentityUser>(null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

            Assert.Throws<ArgumentNullException>(() => new IdentityUserRoleStore<IdentityUser>(userManager, null, null));
            Assert.Throws<ArgumentNullException>(() => new IdentityUserRoleStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), null));
        }

        [Fact]
        public async Task CreateAsync_should_return_role_id()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);


            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());
            var result = await sut.CreateAsync(new Entity.UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            } as object);


            Assert.NotNull(result);
            Assert.NotNull(((Entity.UserRole)result).Id);
        }

        [Fact]
        public async Task DeleteAsync_should_delete_role()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var result = await userManager.CreateAsync(user);
            Assert.True(result.Succeeded);
            
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);

            result = await userManager.AddToRoleAsync(user, role.Name);
            Assert.True(result.Succeeded);

            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());
            await sut.DeleteAsync($"{user.Id}@{role.Id}");

            var roles = await userManager.GetRolesAsync(user);
            Assert.Empty(roles);
        }

        [Fact]
        public async Task UdpateAsync_should_update_role()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var result = await userManager.CreateAsync(user);
            Assert.True(result.Succeeded);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);

            result = await userManager.AddToRoleAsync(user, role.Name);
            Assert.True(result.Succeeded);

            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());
            await sut.UpdateAsync(new Entity.UserRole
            {
                Id = $"{user.Id}@{role.Id}",
                UserId = user.Id,
                RoleId = role.Id,
            } as object);

            var roles = await userManager.GetRolesAsync(user);
            Assert.Single(roles);
        }


        [Fact]
        public async Task GetAsync_by_page_request_should_find_role_roles()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);

            await userManager.AddToRoleAsync(user, role.Name);

            role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);
            await userManager.AddToRoleAsync(user, role.Name);

            role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);
            await userManager.AddToRoleAsync(user, role.Name);

            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var rolesResult = await sut.GetAsync(new PageRequest
            {
                Filter = $"UserId eq '{user.Id}'",
                Take = 1
            });

            Assert.NotNull(rolesResult);
            Assert.Equal(3, rolesResult.Count);
            Assert.Single(rolesResult.Items);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_role()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            var roleResult = await roleManager.CreateAsync(role);
            Assert.True(roleResult.Succeeded);

            await userManager.AddToRoleAsync(user, role.Name);

            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var result = await sut.GetAsync($"{role.NormalizedName}@{user.Id}", null);

            Assert.NotNull(result);
        }
    }
}
