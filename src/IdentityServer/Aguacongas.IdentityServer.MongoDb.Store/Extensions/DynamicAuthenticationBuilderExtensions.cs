// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.MongoDb.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IMvcBuilder"/> extensions
    /// </summary>
    public static class DynamicAuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds the mongo database store.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static DynamicAuthenticationBuilder AddTheIdServerEntityMongoDbStore(this DynamicAuthenticationBuilder builder)
        {
            return builder.AddTheIdServerEntityMongoDbStore<SchemeDefinition>();
        }

        /// <summary>
        /// Adds the identity server admin.
        /// </summary>
        /// <typeparam name="TSchemeDefinition">The type of the scheme definition.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static DynamicAuthenticationBuilder AddTheIdServerEntityMongoDbStore<TSchemeDefinition>(this DynamicAuthenticationBuilder builder)
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            builder.AddTheIdServerStore<TSchemeDefinition>();
            var services = builder.Services;
            services.AddTransient<IAdminStore<ExternalProvider>>(p =>
            {
                var store = p.GetRequiredService<CacheAdminStore<AdminStore<ExternalProvider>, ExternalProvider>>();

                return new NotifyChangedExternalProviderStore<CacheAdminStore<AdminStore<ExternalProvider>, ExternalProvider>>(
                    store,
                    p.GetRequiredService<IProviderClient>(),
                    new PersistentDynamicManager<SchemeDefinition>(
                        p.GetRequiredService<IAuthenticationSchemeProvider>(),
                        p.GetRequiredService<OptionsMonitorCacheWrapperFactory>(),
                        new DynamicProviderStore<SchemeDefinition>(
                            store,
                            p.GetRequiredService<IAuthenticationSchemeOptionsSerializer>()),
                        builder.HandlerTypes),
                    p.GetRequiredService<IAuthenticationSchemeOptionsSerializer>());
            });
            return builder;
        }
    }
}
