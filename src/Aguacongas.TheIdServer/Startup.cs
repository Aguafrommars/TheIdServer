// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using HttpStore = Aguacongas.IdentityServer.Http.Store;

namespace Aguacongas.TheIdServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var isProxy = Configuration.GetValue<bool>("Proxy");

            if (isProxy)
            {
                void configureOptions(HttpStore.IdentityServerOptions options)
                    => Configuration.GetSection("PrivateServerAuthentication").Bind(options);

                services.AddIdentityProviderStore()
                    .AddConfigurationHttpStores(configureOptions)
                    .AddOperationalHttpStores()
                    .AddIdentity<ApplicationUser, IdentityRole>(
                        options => options.SignIn.RequireConfirmedAccount = Configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                    .AddTheIdServerStores(configureOptions)
                    .AddDefaultTokenProviders();
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseDatabaseFromConfiguration(Configuration))
                    .AddIdentityServer4AdminEntityFrameworkStores<ApplicationUser, ApplicationDbContext>()
                    .AddConfigurationEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(Configuration))
                    .AddOperationalEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(Configuration))
                    .AddIdentityProviderStore();

                services.AddIdentity<ApplicationUser, IdentityRole>(
                        options => options.SignIn.RequireConfirmedAccount = Configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            }

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.ConfigureNonBreakingSameSiteCookies()
                .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDefaultSecretParsers()
                .AddDefaultSecretValidators()
                .AddSigningCredentials();

            var authBuilder = services.Configure<ExternalLoginOptions>(Configuration.GetSection("Google"))
                .AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    Configuration.GetSection("ApiAuthentication").Bind(options);
                });

            var externalSettings = Configuration.GetSection("Google").Get<ExternalLoginOptions>();
            if (externalSettings != null)
            {
                authBuilder.AddGoogle(options =>
                 {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5443/signin-google
                     options.ClientId = externalSettings.ClientId;
                     options.ClientSecret = externalSettings.ClientSecret;
                 });
            }

            services.AddControllersWithViews(options =>
                    options.AddIdentityServerAdminFilters())
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddIdentityServerAdmin();
            services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));
        }

        [SuppressMessage("Usage", "ASP0001:Authorization middleware is incorrectly configured.", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app)
        {
            if (!Configuration.GetValue<bool>("Proxy"))
            {
                if (Configuration.GetValue<bool>("Migrate") && Configuration.GetValue<DbTypes>("DbType") != DbTypes.InMemory)
                {
                    using var scope = app.ApplicationServices.CreateScope();
                    var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    configContext.Database.Migrate();

                    var opContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
                    opContext.Database.Migrate();

                    var appcontext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    appcontext.Database.Migrate();
                }

                if (Configuration.GetValue<bool>("Seed"))
                {
                    using var scope = app.ApplicationServices.CreateScope();
                    SeedData.SeedConfiguration(scope);
                    SeedData.SeedUsers(scope);
                }
            }

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                if (Configuration.GetValue<bool>("UseHttps"))
                {
                    app.UseHsts();
                }
            }

            app.UseSerilogRequestLogging();

            if (Configuration.GetValue<bool>("UseHttps"))
            {
                app.UseHttpsRedirection();
            }

            app.UseIdentityServerAdminApi("/api", child =>
                {
                    child.UseOpenApi()
                        .UseSwaggerUi3()
                        .UseCors(configure =>
                        {
                            configure.SetIsOriginAllowed(origin => true)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
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
                });

        }
    }
}