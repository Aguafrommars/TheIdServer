// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Http;

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
                .AddTransient<OAuthDelegatingHandler>()
                .AddTransient(p => new HttpClient(p.GetRequiredService<HttpClientHandler>()));
        }

        public static IServiceCollection AddConfigurationStores(this IServiceCollection services)
        {
            services.TryAddScoped<IMemoryCache>(p => new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>()));
            services.TryAddTransient(typeof(ICache<>), typeof(DefaultCache<>));

            return services.AddTransient<ClientStore>()
                .AddTransient<IClientStore, ValidatingClientStore<ClientStore>>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IExternalProviderKindStore, ExternalProviderKindStore<SchemeDefinition>>();

        }

        public static IServiceCollection AddOperationalStores(this IServiceCollection services)
        {
            return services.AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
                .AddTransient<IPersistentGrantSerializer, PersistentGrantSerializer>()
                .AddTransient<IAuthorizationCodeStore>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IRefreshTokenStore>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IReferenceTokenStore>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IUserConsentStore>(p => p.GetRequiredService<UserConsentStore>())
                .AddTransient<IDeviceFlowStore>(p => p.GetRequiredService<DeviceFlowStore>());
        }
    }
}
