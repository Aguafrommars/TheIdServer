// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Test.IdentityAdminStores
{
    public class IdentityUserLoginStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserLoginStore<IdentityUser>(null, null, null));
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserLoginStore<IdentityUser>(userManager, null, null));
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserLoginStore<IdentityUser>(userManager, context, null));
        }

        [Fact]
        public async Task CreateAsync_should_add_login()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserLoginStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserLoginStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            var user = new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            };
            await userManager.CreateAsync(user).ConfigureAwait(false);

            await sut.CreateAsync(new UserLogin
            {
                UserId = id,
                LoginProvider = id,
                ProviderDisplayName = id,
                ProviderKey = id
            } as object).ConfigureAwait(false);

            Assert.Single(await userManager.GetLoginsAsync(user).ConfigureAwait(false));
        }

        [Fact]
        public async Task CreateAsync_should_throw_on_user_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserLoginStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserLoginStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new UserLogin
            {
                UserId = id,
                LoginProvider = id,
                ProviderDisplayName = id,
                ProviderKey = id
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteAsync_should_throw_when_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserLoginStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserLoginStore<IdentityUser>>>());

            await Assert.ThrowsAsync<DbUpdateException>(() => sut.DeleteAsync("test@test@test")).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateAsync_should_not_be_implemented()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserLoginStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserLoginStore<IdentityUser>>>());


            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new UserLogin
            {
            } as object)).ConfigureAwait(false);

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new UserLogin
            {
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_login()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();

            var id = Guid.NewGuid().ToString();
            await userManager.CreateAsync(new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            }).ConfigureAwait(false);

            var sut = new IdentityUserLoginStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserLoginStore<IdentityUser>>>());

            await sut.CreateAsync(new UserLogin
            {
                UserId = id,
                LoginProvider = id,
                ProviderDisplayName = id,
                ProviderKey = id
            } as object).ConfigureAwait(false);


            var result = await sut.GetAsync($"{id}@{id}@{id}", null).ConfigureAwait(false);

            Assert.NotNull(result);

            result = await sut.GetAsync($"{Guid.NewGuid()}@{id}@{id}", null).ConfigureAwait(false);

            Assert.Null(result);
        }

    }
}
