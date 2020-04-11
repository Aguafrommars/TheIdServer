using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
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
            services
                .AddOptions()
                .AddBaseAddressHttpClient()
                .AddApiAuthorization(options =>
                {
                    var provider = services.BuildServiceProvider();
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    options.ProviderOptions.ConfigurationEndpoint = configuration.GetValue<string>("ConfigurationEndpoint");
                    options.UserOptions.RoleClaim = "role";
                })
                .AddAuthorizationCore(options =>
                {
                    options.AddIdentityServerPolicies();
                })
                .AddIdentityServer4AdminHttpStores(p =>
                {
                    return Task.FromResult(CreateApiHttpClient(p));
                })
                .AddSingleton(p =>
                {
                    var configuration = p.GetRequiredService<IConfiguration>();
                    return configuration.Get<Settings>();
                })
                .AddSingleton<Notifier>()
                .AddSingleton<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>()
                .AddTransient<IAdminStore<User>, UserAdminStore>()
                .AddTransient<IAdminStore<Role>, RoleAdminStore>()
                .AddTransient<IAdminStore<ExternalProvider>, ExternalProviderStore>()
                .AddTransient(p =>
                {
                    var type = Assembly.Load("WebAssembly.Net.Http")
                        .GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
                    return Activator.CreateInstance(type) as HttpMessageHandler;
                })
                .AddTransient<OidcDelegationHandler>()
                .AddHttpClient("oidc")
                .AddHttpMessageHandler<OidcDelegationHandler>();
        }

        private static HttpClient CreateApiHttpClient(IServiceProvider p)
        {
            var httpClient = p.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("oidc");

            var settings = p.GetRequiredService<Settings>();
            var apiUri = new Uri(settings.ApiBaseUrl);

            httpClient.BaseAddress = apiUri;
            return httpClient;
        }

    }
}
