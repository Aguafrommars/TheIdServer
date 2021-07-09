using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Aguacongas.TheIdServer.BlazorApp.Models;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.MAUI
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .UseFormsCompatibility()
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["administratorEmail"] = "aguacongas@gmail.com",
                        ["apiBaseUrl"] = "https://localhost:5443/api",
                        ["authenticationPaths:remoteRegisterPath"] = "/identity/account/register",
                        ["authenticationPaths:remoteProfilePath"] = "/identity/account/manage",
                        ["loggingOptions:minimum"] = "",
                        ["userOptions:roleClaim"] = "role",
                        ["providerOptions:authority"] = "https://localhost:5443/",
                        ["providerOptions:clientId"] = "https://localhost:5443/",
                        ["providerOptions:defaultScopes:0"] = "openid",
                        ["providerOptions:defaultScopes:1"] = "profile",
                        ["providerOptions:defaultScopes:2"] = "theidserveradminapi",
                        ["providerOptions:postLogoutRedirectUri"] = "/authentication/logout-callback",
                        ["providerOptions:redirectUri"] = "/authentication/login-callback",
                        ["providerOptions:responseType"] = "code",
                        ["welcomeContenUrl"] = "/api/welcomefragment",
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddBlazorWebView();
                    var settings = context.Configuration.Get<Settings>();
                    WebAssemblyHostBuilderExtensions.ConfigureServices(services, context.Configuration, settings, settings.ApiBaseUrl);
                });
        }
    }
}