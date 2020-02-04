using Aguacongas.IdentityServer.Http.Store;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationHttpStores(this IServiceCollection services,
            Action<AuthorizationOptions> configureOptions)
        {
            configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            var options = new AuthorizationOptions();
            configureOptions(options);
            services.AddHttpClient(options.HttpClientName).AddHttpMessageHandler<OAuthDelegatingHandler>();

            return services.AddConfigurationHttpStores(p =>
            {
                var factory = p.GetRequiredService<IHttpClientFactory>();
                return Task.FromResult(factory.CreateClient(options.HttpClientName));
            }, configureOptions);
        }

        /// <summary>
        /// Adds the identity server4 HTTP stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationHttpStores(this IServiceCollection services,
        Func<IServiceProvider, Task<HttpClient>> getHttpClient,
        Action<AuthorizationOptions> configureOptions)
        {
            getHttpClient = getHttpClient ?? throw new ArgumentNullException(nameof(getHttpClient));
            configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            return services.Configure(configureOptions)
                .AddHttpContextAccessor()
                .AddIdentityServer4AdminHttpStores(getHttpClient)
                .AddSingleton<OAuthTokenManager>()
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>();
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

    }
}
