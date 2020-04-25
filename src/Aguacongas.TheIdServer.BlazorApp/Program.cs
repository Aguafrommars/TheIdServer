using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
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
            builder.RootComponents.Add<App>("app");            
            ConfigureServices(builder.Services, builder.Configuration, builder.HostEnvironment.BaseAddress);
            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, string baseAddress)
        {
            var providerOptions = configuration.GetSection("ProviderOptions").Get<OidcProviderOptions>();
            var settings = configuration.Get<Settings>();
            services
                .AddOptions()
                .AddOidcAuthentication<RemoteAuthenticationState, RemoteUserAccount>(options =>
                {
                    configuration.GetSection("AuthenticationPaths").Bind(options.AuthenticationPaths);
                    configuration.GetSection("UserOptions").Bind(options.UserOptions);
                    configuration.Bind("ProviderOptions", options.ProviderOptions);
                })
                .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, ClaimsPrincipalFactory>();

            services.AddAuthorizationCore(options =>
                {
                    options.AddIdentityServerPolicies();
                });
                

            services
                .AddIdentityServer4AdminHttpStores(p =>
                {
                    return Task.FromResult(CreateApiHttpClient(p, settings));
                })
                .AddSingleton(new HttpClient { BaseAddress = new Uri(baseAddress) })
                .AddSingleton(p => settings)
                .AddSingleton<Notifier>()
                .AddSingleton<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>()
                .AddTransient<IAdminStore<User>, UserAdminStore>()
                .AddTransient<IAdminStore<Role>, RoleAdminStore>()
                .AddTransient<IAdminStore<ExternalProvider>, ExternalProviderStore>()
                .AddHttpClient("oidc")
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
        }

        private static HttpClient CreateApiHttpClient(IServiceProvider p, Settings settings)
        {
            var httpClient = p.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("oidc");

            var apiUri = new Uri(settings.ApiBaseUrl);
            httpClient.BaseAddress = apiUri;
            return httpClient;
        }

    }
}
