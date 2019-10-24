using Aguacongas.Blazor.Oidc;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtionsions
    {
        public static IServiceCollection AddOidc(this IServiceCollection services, Func<IServiceProvider, Task<AuthorizationOptions>> getAuthorizationOptionsTask)
        {
            return services
                .AddTransient(p => getAuthorizationOptionsTask(p))
                .AddScoped<OidcAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<OidcAuthenticationStateProvider>())
                .AddSingleton<UserStore>();
        }
    }
}
