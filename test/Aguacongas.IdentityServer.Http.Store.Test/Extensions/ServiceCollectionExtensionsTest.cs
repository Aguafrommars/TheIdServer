// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.DependencyInjection;
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
            var provider = new ServiceCollection()
                .AddTransient<HttpClient>()
                .AddTransient<HttpClientHandler>()
                .AddIdentityProviderStore()
                .AddConfigurationHttpStores(options => options.ApiUrl = "http://test")
                .BuildServiceProvider();

            Assert.NotNull(provider.GetService<OAuthDelegatingHandler>());
            Assert.NotNull(provider.GetService<IClientStore>());
            Assert.NotNull(provider.GetService<IResourceStore>());
            Assert.NotNull(provider.GetService<ICorsPolicyService>());
        }

        [Fact]
        public void AddOperationalHttpStores_should_add_http_operational_stores()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddTransient<IPersistentGrantSerializer, PersistentGrantSerializer>()
                .AddIdentityServer4AdminHttpStores(p => Task.FromResult(new HttpClient()))
                .AddOperationalHttpStores()
                .BuildServiceProvider();

            Assert.NotNull(provider.GetService<AuthorizationCodeStore>());
            Assert.NotNull(provider.GetService<RefreshTokenStore>());
            Assert.NotNull(provider.GetService<ReferenceTokenStore>());
            Assert.NotNull(provider.GetService<UserConsentStore>());
            Assert.NotNull(provider.GetService<DeviceFlowStore>());
            Assert.NotNull(provider.GetService<IAuthorizationCodeStore>());
            Assert.NotNull(provider.GetService<IRefreshTokenStore>());
            Assert.NotNull(provider.GetService<IReferenceTokenStore>());
            Assert.NotNull(provider.GetService<IUserConsentStore>());
            Assert.NotNull(provider.GetService<IDeviceFlowStore>());
        }
    }
}
