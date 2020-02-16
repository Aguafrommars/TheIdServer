using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddOptions()
                .AddAuthorizationCore()
                .AddScoped<CustomAuthStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<CustomAuthStateProvider>())
                .AddSingleton<UserStore>();

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
