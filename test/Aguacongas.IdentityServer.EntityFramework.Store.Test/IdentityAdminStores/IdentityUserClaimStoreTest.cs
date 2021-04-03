// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Test.IdentityAdminStores
{
    public class IdentityUserClaimStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserClaimStore<IdentityUser>(null, null, null));
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserClaimStore<IdentityUser>(userManager, null, null));
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserClaimStore<IdentityUser>(userManager, context, null));
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

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new Entity.UserClaim
            {
                UserId = "notfound",
                ClaimType = "test",
                ClaimValue = "test"
            } as object)).ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteAsync_should_not_throw_on_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            await sut.DeleteAsync("0").ConfigureAwait(false);

            Assert.True(true);
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

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            await Assert.ThrowsAsync<DbUpdateException>(() => sut.UpdateAsync(new Entity.UserClaim
            {
                Id = "0"
            } as object)).ConfigureAwait(false);
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

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString(),
                Email = "user@sample.com"
            };
            await userManager.CreateAsync(user).ConfigureAwait(false);
            Assert.Null(await sut.GetAsync("1", null).ConfigureAwait(false));

            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).ConfigureAwait(false);

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

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserClaimStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserClaimStore<IdentityUser>>>());

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString(),
                Email = "user@sample.com"
            };
            await userManager.CreateAsync(user).ConfigureAwait(false);

            await userManager.AddClaimAsync(user, new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).ConfigureAwait(false);

            var result = await sut.GetAsync(new PageRequest
            {
                Filter = $"{nameof(Entity.UserClaim.UserId)} eq '{user.Id}'"
            }).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Single(result.Items);
        }
    }
}
