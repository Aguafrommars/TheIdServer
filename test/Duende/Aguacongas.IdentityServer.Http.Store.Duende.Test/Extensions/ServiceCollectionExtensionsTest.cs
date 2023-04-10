// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using Xunit;
using ISConfiguration = Duende.IdentityServer.Configuration;

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
