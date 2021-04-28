// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddIdentityServerWsFederation(this IMvcBuilder builder)
        {
            builder.Services.AddIdentityServerWsFederation();
            builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
            return builder;
        }
    }
}
