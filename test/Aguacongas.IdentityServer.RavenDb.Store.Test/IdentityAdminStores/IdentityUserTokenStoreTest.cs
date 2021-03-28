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
    public class IdentityUserTokenStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new IdentityUserTokenStore<IdentityUser>(null, null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore)
                .Services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

            Assert.Throws<ArgumentNullException>(() => new IdentityUserTokenStore<IdentityUser>(userManager, null, null));
            Assert.Throws<ArgumentNullException>(() => new IdentityUserTokenStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), null));
        }

        [Fact]
        public async Task CreateAsync_should_return_token_id()
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

            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());
            var result = await sut.CreateAsync(new Entity.UserToken
            {
                UserId = user.Id,
                LoginProvider = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Value = Guid.NewGuid().ToString()
            } as object);


            Assert.NotNull(result);
            Assert.NotNull(((Entity.UserToken)result).Id);
        }

        [Fact]
        public async Task DeleteAsync_should_delete_token()
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

            var loginProvder = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();

            await userManager.SetAuthenticationTokenAsync(user, loginProvder, key, Guid.NewGuid().ToString());

            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());
            await sut.DeleteAsync($"{user.Id}@{loginProvder}@{key}");

            var token = await userManager.GetAuthenticationTokenAsync(user, loginProvder, key);
            Assert.Null(token);
        }

        [Fact]
        public async Task UdpateAsync_should_not_be_implemented()
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


            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new Entity.UserToken()));
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new Entity.UserToken() as object));
        }


        [Fact]
        public async Task GetAsync_by_page_request_should_find_user_tokens()
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

            var loginProvder = Guid.NewGuid().ToString();

            await userManager.SetAuthenticationTokenAsync(user, loginProvder, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            await userManager.SetAuthenticationTokenAsync(user, loginProvder, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            await userManager.SetAuthenticationTokenAsync(user, loginProvder, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());

            var result = await sut.GetAsync(new PageRequest
            {
                Filter = $"UserId eq '{user.Id}'",
                Take = 1
            });

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetAsync_by_id_should_return_token()
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

            var loginProvder = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();

            await userManager.SetAuthenticationTokenAsync(user, loginProvder, key, Guid.NewGuid().ToString());

            var sut = new IdentityUserTokenStore<IdentityUser>(userManager, new ScopedAsynDocumentcSession(documentStore.OpenAsyncSession()), provider.GetRequiredService<ILogger<IdentityUserTokenStore<IdentityUser>>>());

            var result = await sut.GetAsync($"{user.Id}@{loginProvder}@{key}", null);

            Assert.NotNull(result);
        }
    }
}
