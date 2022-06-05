// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTheIdServerPublic(this IApplicationBuilder app, IWebHostEnvironment environment, IConfiguration configuration)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error")
                    .UseHsts();
            }

            app.UseSerilogRequestLogging()
                .UseHttpsRedirection()
                .UseIdentityServerAdminApi("/api", child =>
                {
                    if (configuration.GetValue<bool>("EnableOpenApiDoc"))
                    {
                        child.UseOpenApi()
                            .UseSwaggerUi3(options =>
                            {
                                var settings = configuration.GetSection("SwaggerUiSettings").Get<NSwag.AspNetCore.SwaggerUiSettings>();
                                options.OAuth2Client = settings.OAuth2Client;
                            });
                    }
                    var allowedOrigin = configuration.GetSection("CorsAllowedOrigin").Get<IEnumerable<string>>();
                    if (allowedOrigin != null)
                    {
                        child.UseCors(configure =>
                        {
                            configure.SetIsOriginAllowed(origin => allowedOrigin.Any(o => o == origin))
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                    }
                })
                .UseBlazorFrameworkFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseIdentityServer()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapFallbackToFile("index.html");
                })
                .LoadDynamicAuthenticationConfiguration<SchemeDefinition>();

            return app;
        }
    }
}
