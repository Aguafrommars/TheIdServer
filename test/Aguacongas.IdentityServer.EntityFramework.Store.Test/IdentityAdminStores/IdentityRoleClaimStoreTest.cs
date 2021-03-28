// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Test.IdentityAdminStores
{
    public class IdentityRoleClaimStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleClaimStore<IdentityUser, IdentityRole>(null, null, null));
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, null, null));
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, context, null));
        }

        [Fact]
        public async Task CreateAsync_should_throw_on_error()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new RoleClaim
            {
                RoleId = "notfound",
                ClaimType = "test",
                ClaimValue = "test"
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteAsync_should_throw_on_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            await Assert.ThrowsAsync<DbUpdateException>(() => sut.DeleteAsync("0")).ConfigureAwait(false);

        }

        [Fact]
        public async Task UpdateAsync_should_throw_on_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            await Assert.ThrowsAsync<DbUpdateException>(() => sut.UpdateAsync(new RoleClaim
            {
                Id = "0"
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_claim()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            await roleManager.CreateAsync(role).ConfigureAwait(false);
            Assert.Null(await sut.GetAsync("1", null).ConfigureAwait(false));

            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).ConfigureAwait(false); ;

            var result = await sut.GetAsync("1", null).ConfigureAwait(false);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_should_find_claims()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleClaimStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleClaimStore<IdentityUser, IdentityRole>>>());

            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            await roleManager.CreateAsync(role).ConfigureAwait(false);

            await roleManager.AddClaimAsync(role, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).ConfigureAwait(false); ;

            var result = await sut.GetAsync(new PageRequest
            {
                Filter = $"{nameof(RoleClaim.RoleId)} eq '{role.Id}'"
            }).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Single(result.Items);
        }
    }
}
