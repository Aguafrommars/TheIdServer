// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.TheIdServer.Authentication;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamicAuthenticationBuilderExtensions
    {
        public static DynamicAuthenticationBuilder AddTheIdServerHttpStore(this DynamicAuthenticationBuilder builder, Func<IServiceProvider, Task<HttpClient>> getHttpClient = null)
        {
            return builder.AddTheIdServerHttpStore<SchemeDefinition>(getHttpClient);
        }

        public static DynamicAuthenticationBuilder AddTheIdServerHttpStore<TSchemeDefinition>(this DynamicAuthenticationBuilder builder, Func<IServiceProvider, Task<HttpClient>> getHttpClient = null)
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            builder.AddTheIdServerStore<TSchemeDefinition>();
            if (getHttpClient != null)
            {
                builder.Services.AddIdentityServer4AdminHttpStores(getHttpClient);
            }
            return builder;
        }

        public static DynamicAuthenticationBuilder AddTheIdServerStore(this DynamicAuthenticationBuilder builder)
        {
            return builder.AddTheIdServerStore<SchemeDefinition>();
        }

        public static DynamicAuthenticationBuilder AddTheIdServerStore<TSchemeDefinition>(this DynamicAuthenticationBuilder builder)
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            builder.Services
                .AddTransient<IDynamicProviderStore<TSchemeDefinition>, DynamicProviderStore<TSchemeDefinition>>()
                .AddTransient<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>();

            return builder;
        }
    }
}
