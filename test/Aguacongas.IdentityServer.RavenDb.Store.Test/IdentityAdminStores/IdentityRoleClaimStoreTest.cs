// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityAdminStores
{
    public class IdentityRoleClaimStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleClaimStore<IdentityUser, IdentityRole>(null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            Assert.Throws<ArgumentNullException>(() => new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, null, null));
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), null));
        }

        [Fact]
        public async Task CreateAsync_should_return_claim_id()
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

            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());
            var result = await sut.CreateAsync(new RoleClaim
            {
                ClaimType = "test",
                ClaimValue = "test",
                RoleId = role.Id
            } as object);
            

            Assert.NotNull(result);
            Assert.NotNull(((RoleClaim)result).Id);
        }

        [Fact]
        public async Task DeleteAsync_should_delete_claim()
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
            result = await roleManager.AddClaimAsync(role, new Claim("test", "test"));
            Assert.True(result.Succeeded);

            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());
            await sut.DeleteAsync($"{role.Id}@0");

            var claims = await roleManager.GetClaimsAsync(role);
            Assert.Empty(claims);
        }

        [Fact]
        public async Task UdpateAsync_should_update_claim()
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
            result = await roleManager.AddClaimAsync(role, new Claim("test", "test"));
            Assert.True(result.Succeeded);

            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());
            await sut.UpdateAsync(new RoleClaim
            {
                Id = $"{role.Id}@0",
                ClaimType = Guid.NewGuid().ToString(),
                ClaimValue = Guid.NewGuid().ToString()
            } as object);

            var claims = await roleManager.GetClaimsAsync(role);
            Assert.Single(claims);
            Assert.NotEqual("test", claims.First().Type);
            Assert.NotEqual("test", claims.First().Value);
        }


        [Fact]
        public async Task GetAsync_by_page_request_should_find_role_claims()
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
            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            var claimsResult = await sut.GetAsync(new PageRequest
            {
                Filter = $"RoleId eq '{role.Id}'",
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
            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            var result = await sut.GetAsync($"{role.Id}@1", null);

            Assert.NotNull(result);
        }
    }
}
