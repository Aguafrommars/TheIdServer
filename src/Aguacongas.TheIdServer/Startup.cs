// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
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
                        options => Configuration.GetSection("IdentityOptions").Bind(options))
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                var signalRBuilder = services.AddSignalR(options => Configuration.GetSection("SignalR:HubOptions").Bind(options));
                if (Configuration.GetValue<bool>("SignalR:UseMessagePack"))
                {
                    signalRBuilder.AddMessagePackProtocol();
                }

                var redisConnectionString = Configuration.GetValue<string>("SignalR:RedisConnectionString");
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    signalRBuilder.AddStackExchangeRedis(redisConnectionString, options => Configuration.GetSection("SignalR:RedisOptions").Bind(options));
                }
            }

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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs/chat")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });


            var dynamicAuthBuilder = authBuilder.AddDynamic<SchemeDefinition>();
            if (isProxy)
            {
                dynamicAuthBuilder.AddTheIdServerHttpStore();
            }
            else
            {
                dynamicAuthBuilder.AddEntityFrameworkStore<ConfigurationDbContext>();
            }

            dynamicAuthBuilder.AddGoogle()
                .AddFacebook()
                .AddOpenIdConnect()
                .AddTwitter()
                .AddMicrosoftAccount()
                .AddOAuth("OAuth", options => 
                {
                    options.ClaimActions.MapAll();
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var contextOption = context.Options;
                            if (string.IsNullOrEmpty(contextOption.UserInformationEndpoint))
                            {
                                return;
                            }

                            var request = new HttpRequestMessage(HttpMethod.Get, contextOption.UserInformationEndpoint);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("DynamicAuthProviders-sample", "1.0.0"));

                            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                            var content = await response.Content.ReadAsStringAsync();
                            response.EnsureSuccessStatusCode();

                            using var doc = JsonDocument.Parse(content);

                            context.RunClaimActions(doc.RootElement);
                        }
                    };
                });

            services.Configure<SendGridOptions>(Configuration)
                .AddControllersWithViews(options =>
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
                ConfigureInitialData(app);
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
                    endpoints.MapHub<ProviderHub>("/providerhub");
                    endpoints.MapFallbackToFile("index.html");
                })
                .LoadDynamicAuthenticationConfiguration<SchemeDefinition>();

        }

        private void ConfigureInitialData(IApplicationBuilder app)
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

            if (Configuration.GetValue<bool>("SeedProvider"))
            {
                using var scope = app.ApplicationServices.CreateScope();
                SeedData.SeedProviders(Configuration, scope.ServiceProvider.GetRequiredService<PersistentDynamicManager<SchemeDefinition>>());
            }
        }
    }
}