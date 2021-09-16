// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
#if DUENDE
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using ISConfiguration = Duende.IdentityServer.Configuration;
#else
using IdentityServer4.Services;
using IdentityServer4.Validation;
using ISConfiguration = IdentityServer4.Configuration;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddAddAdminHttpStores_should_add_http_admin_stores()
        {
            var clientConfigurationValidatorMock = new Mock<IClientConfigurationValidator>();
            var eventServiceMock = new Mock<IEventService>();
            var provider = new ServiceCollection()
                .AddTransient<HttpClient>()
                .AddTransient<HttpClientHandler>()
                .AddTransient(p => clientConfigurationValidatorMock.Object)
                .AddTransient(p => eventServiceMock.Object)
                .Configure<ISConfiguration.IdentityServerOptions>(options => options.Caching.ClientStoreExpiration = TimeSpan.FromMinutes(1))
                .AddTransient(p => p.GetRequiredService<IOptions<ISConfiguration.IdentityServerOptions>>().Value)
                .AddIdentityProviderStore()
                .AddAdminHttpStores(options => options.ApiUrl = "http://test")
                .BuildServiceProvider();

            Assert.NotNull(provider.GetService<OAuthDelegatingHandler>());
            Assert.NotNull(provider.GetService<IAdminStore<Client>>());
        }
    }
}
