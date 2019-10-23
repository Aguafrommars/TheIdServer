using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Aguacongas.TheIdServer.BlazorApp
{
    public class Startup
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Startup class")]
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IManageSettings, SettingsManager>()
                .AddSingleton<GridState>()
                .AddIdentityServer4HttpStores(p => p.GetRequiredService<HttpClient>());
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Startup class")]
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
