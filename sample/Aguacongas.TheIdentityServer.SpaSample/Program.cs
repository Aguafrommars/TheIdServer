// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var configuration = builder.Configuration;
            var services = builder.Services;
            services.AddOptions()
                .Configure<RemoteAuthenticationApplicationPathsOptions>(options => configuration.GetSection("AuthenticationPaths").Bind(options))
                .AddOidcAuthentication(options =>
                {
                    configuration.GetSection("AuthenticationPaths").Bind(options.AuthenticationPaths);
                    configuration.GetSection("UserOptions").Bind(options.UserOptions);
                    configuration.Bind("ProviderOptions", options.ProviderOptions);
                })
                .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, ClaimsPrincipalFactory>();

            services.AddAuthorizationCore();

            services.AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri(configuration.GetValue<string>("apiUrl")))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ServerAPI"));

            services.AddHttpClient("TokenApi")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://localhost:5443/api"))
                .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: new[] { "https://localhost:5443/api" },
                        scopes: new[] { "theidservertokenapi" }));

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync(); 
        }
    }
}
