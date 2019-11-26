// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

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
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");            
            
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString))
                .AddIdentityServer4EntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddIdentityProviderStore();

            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews(options =>
                {
                    options.AddIdentityServerAdminFilters();
                })
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddIdentityServerAdmin();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDefaultSecretParsers()
                .AddDefaultSecretValidators();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
#pragma warning disable S112 // General exceptions should never be thrown
                throw new Exception("need to configure key material");
#pragma warning restore S112 // General exceptions should never be thrown
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Id4-Writer", policy =>
                   policy.RequireAssertion(context =>
                       context.User.Identity.Name == "Alice Smith"));
                options.AddPolicy("Id4-Reader", policy =>
                  policy.RequireAssertion(context =>
                      context.User.Identity.IsAuthenticated));
            });
                
            services.AddAuthentication()
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = "https://localhost:5443";
                    options.RequireHttpsMetadata = false;
                    options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
                    options.ApiName = "theidserveradminapi";
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                    options.LegacyAudienceValidation = true;
                })
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });



            services.AddResponseCompression(opts =>
                 {
                     opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                         new[] { "application/octet-stream" });
                 })
                .AddRazorPages(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account");
                });

        }

        [SuppressMessage("Usage", "ASP0001:Authorization middleware is incorrectly configured.", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error")
                    .UseHsts();
            }

            app.UseHttpsRedirection()
                .UseResponseCompression()
                .UseStaticFiles()
                .Map("/admin", child =>
                {
                    child.UseRouting()
                    .UseClientSideBlazorFiles<BlazorApp.Startup>()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToClientSideBlazor<BlazorApp.Startup>("index.html");
                    });
                })
                .UseIdentityServerAdminApi("/api", child =>
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
                .UseIdentityServer()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                }); 
        }
    }
}