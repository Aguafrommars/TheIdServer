// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddWsFederationSample(this WebApplicationBuilder webApplicationBuilder)
        {
            var services = webApplicationBuilder.Services;
            services.AddLogging(options => options.SetMinimumLevel(LogLevel.Debug))
                .AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "aspnetcorewsfed";
                })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = "https://localhost:5443/wsfederation/metadata";
                    options.RequireHttpsMetadata = false;

                    options.Wtrealm = "urn:aspnetcorerp";

                    options.SignOutWreply = "https://localhost:10315";
                    options.SkipUnrecognizedRequests = true;
                });

            return webApplicationBuilder;
        }
    }
}