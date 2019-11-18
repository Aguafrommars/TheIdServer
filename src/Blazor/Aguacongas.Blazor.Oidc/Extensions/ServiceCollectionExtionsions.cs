using Aguacongas.TheIdServer.Blazor.Oidc;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtionsions
    {
        public static IHttpClientBuilder AddOidc(this IServiceCollection services, Func<IServiceProvider, Task<AuthorizationOptions>> getAuthorizationOptionsTask, string httpClientName = "oidc")
        {
            return services
                .AddTransient(p => getAuthorizationOptionsTask(p))
                .AddTransient(p =>
                {
                    var wasmHttpMessageHandlerType =  Assembly.Load("WebAssembly.Net.Http")
                        .GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
                    var constructor = wasmHttpMessageHandlerType.GetConstructor(Array.Empty<Type>());
                    return constructor.Invoke(Array.Empty<object>()) as HttpMessageHandler;
                })
                .AddTransient<OidcDelegationHandler>()
                .AddScoped<OidcAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<OidcAuthenticationStateProvider>())
                .AddSingleton<IUserStore, UserStore>()
                .AddHttpClient(httpClientName)
                .AddHttpMessageHandler<OidcDelegationHandler>();
        }
    }
}
