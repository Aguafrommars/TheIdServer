// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using static IdentityServer4.Stores.CachingCorsPolicyService<Aguacongas.IdentityServer.Store.CorsPolicyService>;

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
                .AddTransient(p => new HttpClient(p.GetRequiredService<HttpClientHandler>()))
                .AddTransient<IIdentityProviderStore, IdentityProviderStore>();
        }

        public static IServiceCollection AddConfigurationStores(this IServiceCollection services)
        {
            services.TryAddScoped<ICache<Client>>(p => new DefaultCache<Client>(new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>())));
            services.TryAddScoped<ICache<IEnumerable<IdentityResource>>>(p => new DefaultCache<IEnumerable<IdentityResource>>(new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>())));
            services.TryAddScoped<ICache<IEnumerable<ApiResource>>>(p => new DefaultCache<IEnumerable<ApiResource>>(new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>())));
            services.TryAddScoped<ICache<IEnumerable<ApiScope>>>(p => new DefaultCache<IEnumerable<ApiScope>>(new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>())));
            services.TryAddScoped<ICache<Resources>>(p => new DefaultCache<Resources>(new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>())));
            services.TryAddScoped<ICache<CorsCacheEntry>>(p => new DefaultCache<CorsCacheEntry>(new MemoryCache(p.GetRequiredService<IOptions<MemoryCacheOptions>>())));

            return services.AddTransient<ClientStore>()
                .AddTransient<ResourceStore>()
                .AddTransient<CorsPolicyService>()
                .AddTransient<ValidatingClientStore<ClientStore>>()
                .AddTransient<IClientStore, CachingClientStore<ValidatingClientStore<ClientStore>>>()
                .AddTransient<ResourceStore>()
                .AddTransient<IResourceStore, CachingResourceStore<ResourceStore>>()
                .AddTransient<CorsPolicyService>()
                .AddTransient<ICorsPolicyService, CachingCorsPolicyService<CorsPolicyService>>();
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
