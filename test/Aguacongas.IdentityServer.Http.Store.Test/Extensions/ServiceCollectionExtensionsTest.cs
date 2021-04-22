// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddConfigurationHttpStores_should_add_http_configuration_stores()
        {
            var clientConfigurationValidatorMock = new Mock<IClientConfigurationValidator>();
            var eventServiceMock = new Mock<IEventService>();
            var provider = new ServiceCollection()
                .AddTransient<HttpClient>()
                .AddTransient<HttpClientHandler>()
                .AddTransient(p => clientConfigurationValidatorMock.Object)
                .AddTransient(p => eventServiceMock.Object)
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => options.Caching.ClientStoreExpiration = TimeSpan.FromMinutes(1))
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddIdentityProviderStore()
                .AddConfigurationHttpStores<SchemeDefinition>(options => options.ApiUrl = "http://test")
                .BuildServiceProvider();

            Assert.NotNull(provider.GetService<OAuthDelegatingHandler>());
            Assert.NotNull(provider.GetService<IClientStore>());
            Assert.NotNull(provider.GetService<IResourceStore>());
            Assert.NotNull(provider.GetService<ICorsPolicyService>());
        }
    }
}
