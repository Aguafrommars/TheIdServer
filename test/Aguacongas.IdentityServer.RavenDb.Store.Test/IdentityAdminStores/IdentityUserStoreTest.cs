// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityAdminStores
{
    public class IdentityUserStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserStore<IdentityUser>(null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

            Assert.Throws<ArgumentNullException>(() => new IdentityUserStore<IdentityUser>(userManager, null, null));
            Assert.Throws<ArgumentNullException>(() => new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), null));
        }

        [Fact]
        public async Task CreateAsync_should_create_user()
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

            var sut = new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserStore<IdentityUser>>>());
            var result = await sut.CreateAsync(new Entity.User
            {
                Email = "exemple@exemple.com",
                UserName = Guid.NewGuid().ToString()
            } as object);


            Assert.NotNull(result);
            Assert.NotNull(((Entity.User)result).Id);
        }

        [Fact]
        public async Task DeleteAsync_should_delete_user()
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

            var sut = new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserStore<IdentityUser>>>());
            await sut.DeleteAsync(user.Id);

            var actual = await userManager.FindByIdAsync(user.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async Task UdpateAsync_should_update_user()
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

            var sut = new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserStore<IdentityUser>>>());
            var newName = Guid.NewGuid().ToString();
            await sut.UpdateAsync(new Entity.User
            {
                Id = user.Id,
                Email = "exemple@exemple.com",
                EmailConfirmed = true,
                UserName = newName,
                SecurityStamp = user.SecurityStamp
            } as object);

            var actual = await userManager.FindByIdAsync(user.Id);
            Assert.Equal(newName, actual.UserName);
        }

        [Fact]
        public async Task GetAsync_by_page_request_should_find_users()
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
                Email = "exemple1@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple3@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);


            user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple3@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            var sut = new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserStore<IdentityUser>>>());

            var result = await sut.GetAsync(new PageRequest
            {
                Filter = "contains(Email, '@exemple.com')",
                Take = 1
            });

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_user()
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
                Email = "exemple1@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple3@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);


            user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "exemple3@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            var sut = new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserStore<IdentityUser>>>());

            var result = await sut.GetAsync(user.Id, null);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_by_id_should_expand_claims_and_roles()
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
                Email = "exemple1@exemple.com",
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };
            var userResult = await userManager.CreateAsync(user);
            Assert.True(userResult.Succeeded);

            await userManager.AddClaimsAsync(user, new[]
            {
                new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            }).ConfigureAwait(false);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var roleName = Guid.NewGuid().ToString();
            await roleManager.CreateAsync(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName
            }).ConfigureAwait(false);

            await userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);

            var sut = new IdentityUserStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserStore<IdentityUser>>>());

            var result = await sut.GetAsync(user.Id, new GetRequest
            {
                Expand = $"{nameof(Entity.User.UserClaims)},{nameof(Entity.User.UserRoles)}"
            });

            Assert.NotNull(result);
            Assert.Single(result.UserClaims);
            Assert.Single(result.UserRoles);
        }
    }
}
