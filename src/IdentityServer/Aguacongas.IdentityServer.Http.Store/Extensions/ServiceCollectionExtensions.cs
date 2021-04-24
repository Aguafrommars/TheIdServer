// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer;
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
                .AddConfigurationStores();
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
