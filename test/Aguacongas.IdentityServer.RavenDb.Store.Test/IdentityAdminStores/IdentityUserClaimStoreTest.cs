// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityAdminStores
{
    public class IdentityUserClaimStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserClaimStore<IdentityUser>(null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            var provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

            Assert.Throws<ArgumentNullException>(() => new IdentityUserClaimStore<IdentityUser>(userManager, null, null));
            Assert.Throws<ArgumentNullException>(() => new IdentityUserClaimStore<IdentityUser>(userManager, documentStore.OpenAsyncSession(), null));
        }

        [Fact]
        public async Task CreateAsync_should_return_claim_id()
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
            var roleResult = await userManager.CreateAsync(user);
            Assert.True(roleResult.Succeeded);

            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, documentStore.OpenAsyncSession(), provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());
            var result = await sut.CreateAsync(new Entity.UserClaim
            {
                ClaimType = "test",
                ClaimValue = "test",
                UserId = user.Id
            } as object);


            Assert.NotNull(result);
            Assert.NotNull(((Entity.UserClaim)result).Id);
        }

        [Fact]
        public async Task DeleteAsync_should_delete_claim()
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
            result = await userManager.AddClaimAsync(user, new Claim("test", "test"));
            Assert.True(result.Succeeded);

            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, documentStore.OpenAsyncSession(), provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());
            await sut.DeleteAsync($"{user.Id}@0");

            var claims = await userManager.GetClaimsAsync(user);
            Assert.Empty(claims);
        }

        [Fact]
        public async Task UdpateAsync_should_update_claim()
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
            result = await userManager.AddClaimAsync(user, new Claim("test", "test"));
            Assert.True(result.Succeeded);

            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, documentStore.OpenAsyncSession(), provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());
            await sut.UpdateAsync(new Entity.UserClaim
            {
                Id = $"{user.Id}@0",
                UserId = user.Id,
                ClaimType = Guid.NewGuid().ToString(),
                ClaimValue = Guid.NewGuid().ToString()
            } as object);

            var claims = await userManager.GetClaimsAsync(user);
            Assert.Single(claims);
            Assert.NotEqual("test", claims.First().Type);
            Assert.NotEqual("test", claims.First().Value);
        }


        [Fact]
        public async Task GetAsync_by_page_request_should_find_role_claims()
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
            var roleResult = await userManager.CreateAsync(user);
            Assert.True(roleResult.Succeeded);
            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, documentStore.OpenAsyncSession(), provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            var claimsResult = await sut.GetAsync(new PageRequest
            {
                Filter = $"UserId eq '{user.Id}'",
                Take = 1
            });

            Assert.NotNull(claimsResult);
            Assert.Equal(3, claimsResult.Count);
            Assert.Single(claimsResult.Items);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_claim()
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
            var roleResult = await userManager.CreateAsync(user);
            Assert.True(roleResult.Succeeded);
            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, documentStore.OpenAsyncSession(), provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            var result = await sut.GetAsync($"{user.Id}@1", null);

            Assert.NotNull(result);
        }
    }
}
