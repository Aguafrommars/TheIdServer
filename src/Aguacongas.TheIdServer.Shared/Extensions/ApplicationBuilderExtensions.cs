﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
#if DUENDE
using Duende.IdentityServer.Hosting;
#endif
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTheIdServer(this IApplicationBuilder app, IWebHostEnvironment environment, IConfiguration configuration)
        {
            var isProxy = configuration.GetValue<bool>("Proxy");
            var dbType = configuration.GetValue<DbTypes>("DbType");

            var disableHttps = configuration.GetValue<bool>("DisableHttps");

            app.UseForwardedHeaders();
            AddForceHttpsSchemeMiddleware(app, configuration);

            if (!isProxy)
            {
                ConfigureInitialData(app, configuration, dbType);
            }

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                if (!disableHttps)
                {
                    app.UseHsts();
                }
            }

            var scope = app.ApplicationServices.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var supportedCulture = scopedProvider.GetRequiredService<ISupportCultures>().CulturesNames.ToArray();


            app.UseRequestLocalization(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture("en");
                    options.SupportedCultures = supportedCulture.Select(c => new CultureInfo(c)).ToList();
                    options.SupportedUICultures = options.SupportedCultures;
                    options.FallBackToParentCultures = true;
                    options.AddInitialRequestCultureProvider(new SetCookieFromQueryStringRequestCultureProvider());
                })
                .UseSerilogRequestLogging();

            if (!disableHttps)
            {
                app.UseHttpsRedirection();
            }

            app.UseBlazorFrameworkFiles()
               .UseStaticFiles()
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
                .UseRouting()
#if DUENDE
                .UseMiddleware<BaseUrlMiddleware>()
                .ConfigureCors();

            new IdentityServerMiddlewareOptions().AuthenticationMiddleware(app);

            app.UseMiddleware<MutualTlsEndpointMiddleware>()
               .UseMiddleware<IdentityServerMiddleware>();
#else
               .UseIdentityServer();
#endif

            if (!isProxy)
            {
                app.UseIdentityServerAdminAuthentication("/providerhub", JwtBearerDefaults.AuthenticationScheme);
            }

            app.UseAuthorization()
                .Use((context, next) =>
                {
                    var service = context.RequestServices;
                    var settings = service.GetRequiredService<Settings>();
                    var request = context.Request;
                    settings.WelcomeContenUrl = $"{request.Scheme}://{request.Host}/api/welcomefragment";
                    var remotePathOptions = service.GetRequiredService<IOptions<RemoteAuthenticationApplicationPathsOptions>>().Value;
                    remotePathOptions.RemoteProfilePath = $"{request.Scheme}://{request.Host}/identity/account/manage";
                    remotePathOptions.RemoteRegisterPath = $"{request.Scheme}://{request.Host}/identity/account/register";
                    return next();
                });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                if (!isProxy)
                {
                    endpoints.MapHub<ProviderHub>("/providerhub");
                }
                endpoints.MapFallbackToPage("/_Host");
            });
            app.LoadDynamicAuthenticationConfiguration<SchemeDefinition>();

            return app;
        }

        private static void AddForceHttpsSchemeMiddleware(IApplicationBuilder app, IConfiguration configuration)
        {
            var forceHttpsScheme = configuration.GetValue<bool>("ForceHttpsScheme");

            if (forceHttpsScheme)
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }
        }

        private static void ConfigureInitialData(IApplicationBuilder app, IConfiguration configuration, DbTypes dbType)
        {
            if (configuration.GetValue<bool>("Migrate") &&
                dbType != DbTypes.InMemory && dbType != DbTypes.RavenDb && dbType != DbTypes.MongoDb)
            {
                using var scope = app.ApplicationServices.CreateScope();
                var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configContext.Database.Migrate();

                var opContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
                opContext.Database.Migrate();

                var appcontext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                appcontext.Database.Migrate();
            }

            if (configuration.GetValue<bool>("Seed"))
            {
                using var scope = app.ApplicationServices.CreateScope();
                SeedData.SeedConfiguration(scope, configuration);
                SeedData.SeedUsers(scope, configuration);
            }

        }

    }
}
