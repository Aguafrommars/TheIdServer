using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "entry point")]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .AddAuthorizationCore(options =>
                {
                    options.AddIdentityServerPolicies();
                })
                .AddIdentityServer4AdminHttpStores(async p =>
                {
                    return await CreateApiHttpClient(p)
                        .ConfigureAwait(false);
                })
                .AddOidc(async p => {
                    var settings = await p.GetRequiredService<Task<Settings>>()
                        .ConfigureAwait(false);
                    settings.RedirectUri = p.GetRequiredService<NavigationManager>().BaseUri;
                    return settings;
                });

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
