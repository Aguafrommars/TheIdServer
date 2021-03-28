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
    public class IdentityRoleStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleStore<IdentityUser, IdentityRole>(null, null, null));
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, null, null));
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            Assert.Throws<ArgumentNullException>(() => new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, context, null));
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
            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());

            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
            await roleManager.CreateAsync(role).ConfigureAwait(false);

            await Assert.ThrowsAsync<IdentityException>(() => sut.CreateAsync(new Role
            {
                Id = role.Id,
                Name = role.Name
            } as object)).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateAsync_should_update_role()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());

            var name = Guid.NewGuid().ToString();
            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = name
            };
            await roleManager.CreateAsync(role).ConfigureAwait(false);

            await sut.UpdateAsync(new Role
            {
                Id = role.Id,
                Name = Guid.NewGuid().ToString()
            } as object).ConfigureAwait(false);

            var actual = await sut.GetAsync(role.Id, null);

            Assert.NotEqual(name, actual.Name);
        }

        [Fact]
        public async Task UpdateAsync_should_throw_on_role_not_found()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext<IdentityUser>>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
                .Services.BuildServiceProvider();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<IdentityDbContext<IdentityUser>>();
            var sut = new IdentityRoleStore<IdentityUser, IdentityRole>(roleManager, context, provider.GetRequiredService<ILogger<IdentityRoleStore<IdentityUser, IdentityRole>>>());
            await Assert.ThrowsAsync< IdentityException>(() => sut.UpdateAsync(new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            })).ConfigureAwait(false);
        }
    }
}
