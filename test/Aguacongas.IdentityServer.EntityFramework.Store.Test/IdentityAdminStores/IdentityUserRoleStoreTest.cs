// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
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
    public class IdentityUserRoleStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserRoleStore<IdentityUser>(null, null, null));
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserRoleStore<IdentityUser>(userManager, null, null));
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityUserRoleStore<IdentityUser>(userManager, context, null));
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
            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await userManager.CreateAsync(new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            }).ConfigureAwait(false);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole
            {
                Id = id,
                Name = id
            }).ConfigureAwait(false);

            await sut.CreateAsync(new UserRole
            {
                RoleId = id,
                UserId = id
            } as object).ConfigureAwait(false);

            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new UserRole
            {
                RoleId = id,
                UserId = id
            })).ConfigureAwait(false);
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
            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new UserRole
            {
                RoleId = id,
                UserId = id
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateAsync_should_throw_on_role_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await userManager.CreateAsync(new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            }).ConfigureAwait(false);

            await Assert.ThrowsAsync<DbUpdateException>(() => sut.CreateAsync(new UserRole
            {
                RoleId = id,
                UserId = id
            })).ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteAsync_should_throw_on_error()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await userManager.CreateAsync(new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            }).ConfigureAwait(false);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole
            {
                Id = id,
                Name = id
            }).ConfigureAwait(false);

            await Assert.ThrowsAsync<IdentityException>(() => sut.DeleteAsync($"{id}@{id}")).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateAsync_should_replace_role()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var id = Guid.NewGuid().ToString();
            await userManager.CreateAsync(new IdentityUser
            {
                Email = "test@sample.com",
                Id = id,
                UserName = id
            }).ConfigureAwait(false);

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole
            {
                Id = id,
                Name = id
            }).ConfigureAwait(false);

            var newId = Guid.NewGuid().ToString();
            await roleManager.CreateAsync(new IdentityRole
            {
                Id = newId,
                Name = newId
            }).ConfigureAwait(false);

            await sut.CreateAsync(new UserRole
            {
                RoleId = id,
                UserId = id
            } as object).ConfigureAwait(false);


            await sut.UpdateAsync(new UserRole
            {
                Id = $"{id}@{id}",
                RoleId = newId,
                UserId = id
            } as object).ConfigureAwait(false);

            var removed = await sut.GetAsync($"{id}@{id}", null).ConfigureAwait(false);
            Assert.Null(removed);

            var added = await sut.GetAsync($"{id}@{newId}", null).ConfigureAwait(false);
            Assert.NotNull(added);
        }

        [Fact]
        public async Task GetAsync_should_find_user_role()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityUserRoleStore<IdentityUser>(userManager, context, provider.GetRequiredService<ILogger<IdentityUserRoleStore<IdentityUser>>>());

            var result = await sut.GetAsync(new PageRequest()).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }
    }


}
