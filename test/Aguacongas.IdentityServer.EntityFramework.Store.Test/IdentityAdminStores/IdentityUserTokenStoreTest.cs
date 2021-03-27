// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Test.IdentityAdminStores
{
    public class IdentityUserTokenStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserTokenStore<IdentityUser>(null, null, null));
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserTokenStore<IdentityUser>(userManager, null, null));
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserTokenStore<IdentityUser>(userManager, context, null));
        }

        [Fact]
        public async Task CreateAsync_should_set_token()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            var user = new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            };
            await userManager.CreateAsync(user).ConfigureAwait(false);
            
            await sut.CreateAsync(new UserToken
            {
                UserId = id,
                LoginProvider = id,
                Name = id,
                Value = id
            } as object).ConfigureAwait(false);

            Assert.NotNull(await userManager.GetAuthenticationTokenAsync(user, id, id).ConfigureAwait(false));
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
            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new UserToken
            {
                UserId = id,
                LoginProvider = id,
                Name = id,
                Value = id
            })).ConfigureAwait(false);
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
            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());


            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new UserToken
            {
            } as object)).ConfigureAwait(false);

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new UserToken
            {
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_token()
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

            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());

            await sut.CreateAsync(new UserToken
            {
                UserId = id,
                LoginProvider = id,
                Name = id,
                Value = id
            } as object).ConfigureAwait(false);


            var result = await sut.GetAsync($"{id}@{id}@{id}", null).ConfigureAwait(false);

            Assert.NotNull(result);

            result = await sut.GetAsync($"{Guid.NewGuid()}@{id}@{id}", null).ConfigureAwait(false);

            Assert.Null(result);
        }
    }


}
