// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.WsFederationSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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
                    options.MetadataAddress = "https://localhost:5001/wsfed";
                    options.RequireHttpsMetadata = false;

                    options.Wtrealm = "urn:aspnetcorerp";

                    options.SignOutWreply = "https://localhost:10315";
                    options.SkipUnrecognizedRequests = true;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage()
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()                
                .UseEndpoints(enpoints => enpoints.MapDefaultControllerRoute());
        }
    }
}