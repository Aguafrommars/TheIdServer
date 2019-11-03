using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    public class Startup
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Startup class")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Id4-Writer", policy =>
                   policy.RequireAssertion(context =>
                       context.User.Identity.Name == "Alice Smith"));
            });

            services
                .AddIdentityServer4HttpStores(async p =>
                {
                    return await CreateApiHattpClient(p)
                        .ConfigureAwait(false);
                })
                .AddAuthorizationCore()
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
                .AddSingleton<Notifier>();
        }

        private static async Task<HttpClient> CreateApiHattpClient(IServiceProvider p)
        {
            var httpClient = p.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("oidc");
            var settingsTask = p.GetRequiredService<Task<Settings>>();
            var settings = await settingsTask
                .ConfigureAwait(false);
            var apiUri = new Uri(settings.ApiBaseUrl);
            if (!apiUri.IsAbsoluteUri)
            {
                var navigationManager = p.GetRequiredService<NavigationManager>();
                var baseUri = new Uri(navigationManager.BaseUri);
                var host = baseUri.IsDefaultPort ? baseUri.Host : $"{baseUri.Host}:{baseUri.Port}";
                apiUri = new Uri($"{baseUri.Scheme}://{host}{settings.ApiBaseUrl}");
            }
            httpClient.BaseAddress = apiUri;
            return httpClient;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Startup class")]
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
