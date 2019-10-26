using Aguacongas.Blazor.Oidc;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtionsions
    {
        public static IHttpClientBuilder AddOidc(this IServiceCollection services, Func<IServiceProvider, Task<AuthorizationOptions>> getAuthorizationOptionsTask, string httpClientName = "oidc")
        {
            return services
                .AddTransient<OidcWebAssemblyHttpMessageHandler>()
                .AddTransient<WebAssemblyHttpMessageHandler>()
                .AddTransient<OidcDelegationHandler>()
                .AddTransient(p => getAuthorizationOptionsTask(p))
                .AddScoped<OidcAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<OidcAuthenticationStateProvider>())
                .AddSingleton<IUserStore, UserStore>()
                .AddHttpClient(httpClientName)
                .AddHttpMessageHandler<OidcDelegationHandler>();
        }
    }
}
