using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "entry point")]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .AddBaseAddressHttpClient()
                .AddApiAuthorization(options =>
                {
                    options.ProviderOptions.ConfigurationEndpoint = "settings.json";
                    options.UserOptions.RoleClaim = "role";
                })
                .AddAuthorizationCore(options =>
                {
                    options.AddIdentityServerPolicies();
                })
                .AddIdentityServer4AdminHttpStores(async p =>
                {
                    return await CreateApiHttpClient(p)
                        .ConfigureAwait(false);
                })
                .AddTransient(p =>
                {
                    var type = Assembly.Load("WebAssembly.Net.Http")
                        .GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
                    return Activator.CreateInstance(type) as HttpMessageHandler;
                })
                .AddTransient<OidcDelegationHandler>()
                .AddHttpClient("oidc")
                .AddHttpMessageHandler<OidcDelegationHandler>();

            services.AddSingleton(async p =>
                {
                    var httpClient = p.GetRequiredService<HttpClient>();
                    return await httpClient.GetJsonAsync<Settings>("settings.json")
                        .ConfigureAwait(false);
                })
                .AddSingleton<Notifier>()
                .AddTransient<IAdminStore<User>, UserAdminStore>()
                .AddTransient<IAdminStore<Role>, RoleAdminStore>();
        }

        private static async Task<HttpClient> CreateApiHttpClient(IServiceProvider p)
        {
            var httpClient = p.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("oidc");
            var settingsTask = p.GetRequiredService<Task<Settings>>();
            var settings = await settingsTask
                .ConfigureAwait(false);
            var apiUri = new Uri(settings.ApiBaseUrl);

            httpClient.BaseAddress = apiUri;
            return httpClient;
        }

    }
}
