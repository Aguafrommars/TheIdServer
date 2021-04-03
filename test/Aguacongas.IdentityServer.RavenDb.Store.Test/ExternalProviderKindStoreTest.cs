// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test
{
    public class ExternalProviderKindStoreTest
    {
        [Fact]
        public void Constuctor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new ExternalProviderKindStore(null, null));
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            var provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();

            Assert.Throws<ArgumentNullException>(() => new ExternalProviderKindStore(manager, null));
        }

        [Fact]
        public async Task GetAsync_by_page_request_should_find_handlers_types()
        {
            using var documentStore = new RavenDbTestDriverWrapper().GetDocumentStore();
            var services = new ServiceCollection()
                .AddLogging();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRavenDbStores(p => documentStore);

            services.AddSingleton(p => documentStore)
                .AddAuthentication()
                .AddDynamic<SchemeDefinition>()
                .AddRavenDbStore()
                .AddGoogle();

            IServiceProvider provider = services.AddIdentityServer4AdminRavenDbkStores<IdentityUser, IdentityRole>(p => documentStore).BuildServiceProvider();
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;

            var manager = provider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>();

            await manager.AddAsync(new SchemeDefinition
            {
                DisplayName = Guid.NewGuid().ToString(),
                Options = new GoogleOptions(),
                HandlerType = typeof(GoogleHandler),
                Scheme = $"scheme-{Guid.NewGuid()}"
            });

            var sut = new ExternalProviderKindStore(manager, provider.GetService<IAuthenticationSchemeOptionsSerializer>());

            var rolesResult = await sut.GetAsync(new PageRequest
            {
                Take = 1
            });

            Assert.NotNull(rolesResult);
            Assert.Equal(1, rolesResult.Count);
            Assert.Single(rolesResult.Items);
        }
    }
}
