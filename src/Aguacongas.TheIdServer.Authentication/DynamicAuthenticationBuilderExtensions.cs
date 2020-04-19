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
            var services = builder.Services;
            services
                .AddTransient<IDynamicProviderStore<TSchemeDefinition>, DynamicProviderStore<TSchemeDefinition>>()
                .AddTransient<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>();
            if (getHttpClient != null)
            {
                services.AddIdentityServer4AdminHttpStores(getHttpClient);
            }
            return builder;
        }
    }
}
