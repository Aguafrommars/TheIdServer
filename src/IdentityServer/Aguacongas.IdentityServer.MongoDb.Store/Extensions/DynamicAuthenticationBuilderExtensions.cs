// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.TheIdServer.Authentication;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="getDatabase">The get database.</param>
        /// <returns></returns>
        public static DynamicAuthenticationBuilder AddMongoDbStore(this DynamicAuthenticationBuilder builder, Func<IServiceProvider, IMongoDatabase> getDatabase = null)
        {
            return builder.AddMongoDbStore<SchemeDefinition>(getDatabase);
        }

        /// <summary>
        /// Adds the identity server admin.
        /// </summary>
        /// <typeparam name="TSchemeDefinition">The type of the scheme definition.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="getDatabase">The get database.</param>
        /// <returns></returns>
        public static DynamicAuthenticationBuilder AddMongoDbStore<TSchemeDefinition>(this DynamicAuthenticationBuilder builder, Func<IServiceProvider, IMongoDatabase> getDatabase = null)
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            var services = builder.Services;
            services
                .AddTransient<IDynamicProviderStore<TSchemeDefinition>, DynamicProviderStore<TSchemeDefinition>>()
                .AddTransient<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>();

            services.AddIdentityServer4AdminMongoDbStores(getDatabase);
            return builder;
        }
    }
}
