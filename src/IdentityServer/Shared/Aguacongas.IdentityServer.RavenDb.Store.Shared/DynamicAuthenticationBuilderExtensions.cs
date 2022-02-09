// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.RavenDb.Store;
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
        public static DynamicAuthenticationBuilder AddTheIdServerStoreRavenDbStore(this DynamicAuthenticationBuilder builder)
        {
            return builder.AddTheIdServerStoreRavenDbStore<SchemeDefinition>();
        }

        /// <summary>
        /// Adds the identity server admin.
        /// </summary>
        /// <typeparam name="TSchemeDefinition">The type of the scheme definition.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="getDatabase">The get database.</param>
        /// <returns></returns>
        public static DynamicAuthenticationBuilder AddTheIdServerStoreRavenDbStore<TSchemeDefinition>(this DynamicAuthenticationBuilder builder)
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            return builder.AddTheIdServerStore<TSchemeDefinition>()
                .AddNotifyChangedExternalProviderStore<CacheAdminStore<AdminStore<ExternalProvider>, ExternalProvider>>();
        }
    }
}
