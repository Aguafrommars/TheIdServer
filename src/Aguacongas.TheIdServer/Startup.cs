// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
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


            authBuilder.AddDynamic<SchemeDefinition>()
                .AddEntityFrameworkStore<ConfigurationDbContext>()
                .AddGoogle()
                .AddJwtBearer()
                .AddFacebook()
                .AddOpenIdConnect()
                .AddTwitter()
                .AddWsFederation()
                .AddOAuth("Github", "Github", options =>
                {
                    // You can defined default configuration for managed handlers.
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.ClaimsIssuer = "OAuth2-Github";
                    // Retrieving user information is unique to each provider.
                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                    options.ClaimActions.MapJsonKey("urn:github:name", "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
                    options.ClaimActions.MapJsonKey("urn:github:url", "url");
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            // Get the GitHub user
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            // A user-agent header is required by GitHub. See (https://developer.github.com/v3/#user-agent-required)
                            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("DynamicAuthProviders-sample", "1.0.0"));

                            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                            var content = await response.Content.ReadAsStringAsync();
                            response.EnsureSuccessStatusCode();

                            using var doc = JsonDocument.Parse(content);

                            context.RunClaimActions(doc.RootElement);
                        }
                    };
                });

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
                    SeedData.SeedProviders(Configuration, scope.ServiceProvider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>());
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
                    if (Configuration.GetValue<bool>("EnableOpenApiDoc"))
                    {
                        child.UseOpenApi()
                            .UseSwaggerUi3(options =>
                            {
                                var settings = Configuration.GetSection("SwaggerUiSettings").Get<NSwag.AspNetCore.SwaggerUiSettings>();
                                options.OAuth2Client = settings.OAuth2Client;
                            });
                    }
                    var allowedOrigin = Configuration.GetSection("CorsAllowedOrigin").Get<IEnumerable<string>>();
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

        }
    }
}