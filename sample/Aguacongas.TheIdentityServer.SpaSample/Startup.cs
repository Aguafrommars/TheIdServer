using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore()
                .AddScoped<CustomAuthStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<CustomAuthStateProvider>())
                .AddSingleton<UserStore>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
