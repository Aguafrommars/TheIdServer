// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding TheIdServer stores.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds TheIdServer stores.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">configureOptions</exception>
        public static IdentityBuilder AddTheIdServerStores(this IdentityBuilder builder, Action<IdentityServerOptions> configureOptions)
        {
            configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            var options = new IdentityServerOptions();
            configureOptions(options);
            var services = builder.Services;
            services
                .AddHttpClient(options.HttpClientName)
                .ConfigurePrimaryHttpMessageHandler(p => p.GetRequiredService<HttpClientHandler>())
                .AddHttpMessageHandler<OAuthDelegatingHandler>();

            builder.AddTheIdServerStores(provider => provider.CreateApiHttpClient(options));
            return builder;
        }

        /// <summary>
        /// Adds TheIdServer stores.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="getHttpClient">The get HTTP client.</param>
        /// <returns></returns>
        public static IdentityBuilder AddTheIdServerStores(this IdentityBuilder builder, Func<IServiceProvider, Task<HttpClient>> getHttpClient)
        {
            builder.Services.AddTheIdServerStores(builder.UserType, builder.RoleType, getHttpClient);
            return builder;
        }
    }
}