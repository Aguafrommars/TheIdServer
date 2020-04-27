using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the identity provider store in DI container.
        /// </summary>
        /// <param name="services">The collection of service.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityProviderStore(this IServiceCollection services)
        {
            return services
                .AddSingleton(p => new OAuthTokenManager(p.GetRequiredService<HttpClient>(), p.GetRequiredService<IOptions<IdentityServerOptions>>()))
                .AddSingleton<HubConnectionFactory>()
                .AddTransient(p => new HubHttpMessageHandlerAccessor { Handler = p.GetRequiredService<HttpClientHandler>() })
                .AddTransient<OAuthDelegatingHandler>()
                .AddTransient(p => new HttpClient(p.GetRequiredService<HttpClientHandler>()))
                .AddTransient<IIdentityProviderStore, IdentityProviderStore>();
        }
    }
}
