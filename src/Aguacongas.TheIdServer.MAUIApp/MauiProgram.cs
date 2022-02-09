// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.MAUIApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;

namespace Aguacongas.TheIdServer.MAUIApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            var configuration = builder.Configuration;
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["apiBaseUrl"] = "https://theidserver-duende.herokuapp.com/api",
                ["authenticationPaths:remoteRegisterPath"] = "/identity/account/register",
                ["authenticationPaths:remoteProfilePath"] = "/identity/account/manage",
                ["userOptions:roleClaim"] = "role",
                ["providerOptions:authority"] = "https://theidserver-duende.herokuapp.com",
                ["providerOptions:clientId"] = "theidserveradmin",
                ["providerOptions:defaultScopes"] = @" [
      ""openid"",
      ""profile"",
      ""theidserveradminapi""
    ]",
                ["providerOptions:postLogoutRedirectUri"] = "https://theidserver-duende.herokuapp.com/authentication/logout-callback",
                ["providerOptions:postLogoutRedirectUri"] = "https://theidserver-duende.herokuapp.com/authentication/login-callback",
                ["providerOptions:responseType"] = "code",
                ["welcomeContenUrl"] = "https://theidserver-duende.herokuapp.com/api/welcomefragment",
                ["settingsOptions:typeName"] = "Aguacongas.TheIdServer.BlazorApp.Models.ServerConfig, Aguacongas.TheIdServer.BlazorApp.Infrastructure",
                ["settingsOptions:apiUrl"] = "https://theidserver-duende.herokuapp.com/api/api/configuration",
                ["menuOptions:showSettings"] = "true"
            });

            var settings = configuration.Get<Settings>();

            var services = builder.Services.AddBlazorWebView();
            WebAssemblyHostBuilderExtensions.ConfigureServices(services, configuration, settings);
                       

            return builder.Build();
        }
    }
}