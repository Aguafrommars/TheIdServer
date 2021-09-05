// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre

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
        /// <returns></returns>
        public static IMvcBuilder AddIdentityServerWsFederation(this IMvcBuilder builder)
        {
            builder.Services.AddIdentityServerWsFederation();
            builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
            return builder;
        }
    }
}
