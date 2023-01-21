// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre

using Aguacongas.IdentityServer.WsFederation;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds identity server WS-Federation controller.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configuration">Configuration to bind to <see cref="WsFederationOptions"/></param>
        /// <returns></returns>
        public static IMvcBuilder AddIdentityServerWsFederation(this IMvcBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddIdentityServerWsFederation(configuration);
            builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
            return builder;
        }

        /// <summary>
        /// Adds identity server WS-Federation controller.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="options">Options to configure metadata</param>
        /// <returns></returns>
        public static IMvcBuilder AddIdentityServerWsFederation(this IMvcBuilder builder, WsFederationOptions options = null)
        {
            builder.Services.AddIdentityServerWsFederation(options);
            builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
            return builder;
        }
    }
}
