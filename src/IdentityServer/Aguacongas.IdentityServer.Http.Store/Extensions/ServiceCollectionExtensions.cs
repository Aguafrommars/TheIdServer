using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Http.Store;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Localization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationHttpStores(this IServiceCollection services,
            Action<IdentityServerOptions> configureOptions)
        {
            configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            var options = new IdentityServerOptions();
            configureOptions(options);
            services.AddHttpClient(options.HttpClientName)
                .ConfigurePrimaryHttpMessageHandler((p => p.GetRequiredService<HttpClientHandler>()))
                .AddHttpMessageHandler<OAuthDelegatingHandler>();

            return services.AddConfigurationHttpStores(p => p.CreateApiHttpClient(options), configureOptions);
        }

        /// <summary>
        /// Adds the identity server4 HTTP stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationHttpStores(this IServiceCollection services,
        Func<IServiceProvider, Task<HttpClient>> getHttpClient,
        Action<IdentityServerOptions> configureOptions)
        {
            getHttpClient = getHttpClient ?? throw new ArgumentNullException(nameof(getHttpClient));
            configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            return services.Configure(configureOptions)
                .AddIdentityServer4AdminHttpStores(getHttpClient)
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IStringLocalizerFactory, StringLocalizerFactory>()
                .AddTransient<ISupportCultures, StringLocalizerFactory>();
        }

        /// <summary>
        /// Adds the operational HTTP stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalHttpStores(this IServiceCollection services)
        {
            return services.AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
                .AddTransient<IAuthorizationCodeStore>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IRefreshTokenStore>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IReferenceTokenStore>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IUserConsentStore>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IGetAllUserConsentStore, GetAllUserConsentStore>()
                .AddTransient<IDeviceFlowStore>(p => p.GetRequiredService<DeviceFlowStore>());
        }

        /// <summary>
        /// Creates the API HTTP client.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static Task<HttpClient> CreateApiHttpClient(this IServiceProvider provider, IdentityServerOptions options)
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient(options.HttpClientName);
            client.BaseAddress = new Uri(options.ApiUrl);
            return Task.FromResult(client);
        }
    }
}
