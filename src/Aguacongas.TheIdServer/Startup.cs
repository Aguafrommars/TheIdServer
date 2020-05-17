// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Auth = Aguacongas.TheIdServer.Authentication;

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
                AddProxyServices(services);
            }
            else
            {
                AddDefaultServices(services);
            }

            services.Configure<ForwardedHeadersOptions>(options => Configuration.GetSection("ForwardedHeadersOptions").Bind(options))
                .ConfigureNonBreakingSameSiteCookies()
                .AddIdentityServer(options => Configuration.GetSection("IdentityServerOptions").Bind(options))
                .AddAspNetIdentity<ApplicationUser>()
                .AddDefaultSecretParsers()
                .AddDefaultSecretValidators()
                .AddSigningCredentials()
                .AddProfileService<ProfileService<ApplicationUser>>();

            services.AddTransient(p =>
                {
                    var handler = new HttpClientHandler();
                    if (Configuration.GetValue<bool>("DisableStrictSsl"))
                    {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, policy) => true;
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                    }
                    return handler;
                })
                .AddHttpClient(OAuth2IntrospectionDefaults.BackChannelHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(p => p.GetRequiredService<HttpClientHandler>());

            services.Configure<ExternalLoginOptions>(Configuration.GetSection("Google"))
                .AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    Configuration.GetSection("ApiAuthentication").Bind(options);
                    if (Configuration.GetValue<bool>("DisableStrictSsl"))
                    {
                        options.JwtBackChannelHandler = new HttpClientHandler
                        {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                            ServerCertificateCustomValidationCallback = (message, cert, chain, policy) => true
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                        };
                    }

                    static string tokenRetriever(HttpRequest request)
                    {
                        var accessToken = TokenRetrieval.FromQueryString()(request);

                        var path = request.Path;
                        if (path.StartsWithSegments("/providerhub") && !string.IsNullOrEmpty(accessToken))
                        {
                            return accessToken;
                        }
                        return TokenRetrieval.FromAuthorizationHeader()(request);
                    }

                    options.TokenRetriever = tokenRetriever;
                });


            var mvcBuilder = services.Configure<SendGridOptions>(Configuration)
                .AddControllersWithViews(options =>
                    options.AddIdentityServerAdminFilters())
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            
            if (isProxy)
            {
                mvcBuilder.AddIdentityServerAdmin<ApplicationUser, Auth.SchemeDefinition>()
                    .AddTheIdServerHttpStore();
            }
            else
            {
                mvcBuilder.AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>()
                    .AddEntityFrameworkStore<ConfigurationDbContext>();
            }
            services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));
        }

        [SuppressMessage("Usage", "ASP0001:Authorization middleware is incorrectly configured.", Justification = "<Pending>")]
        public void Configure(IApplicationBuilder app)
        {
            var isProxy = Configuration.GetValue<bool>("Proxy");
            var disableHttps = Configuration.GetValue<bool>("DisableHttps");
            if (!isProxy)
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
                if (!disableHttps)
                {
                    app.UseHsts();
                }
            }

            app.UseSerilogRequestLogging();

            if (!disableHttps)
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
                .UseAuthentication();

            if (!isProxy)
            {
                app.UseIdentityServerAdminAuthentication("/providerhub", JwtBearerDefaults.AuthenticationScheme);
            }

            app.UseIdentityServer()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapDefaultControllerRoute();
                    if (!isProxy)
                    {
                        endpoints.MapHub<ProviderHub>("/providerhub");
                    }

                    endpoints.MapFallbackToFile("index.html");
                });

            if (isProxy)
            {
                app.LoadDynamicAuthenticationConfiguration<Auth.SchemeDefinition>();
            }
            else
            {
                app.LoadDynamicAuthenticationConfiguration<SchemeDefinition>();
            }


            var scope = app.ApplicationServices.CreateScope();
            scope.ServiceProvider.GetRequiredService<ISchemeChangeSubscriber>().Subscribe();
        }

        private void AddDefaultServices(IServiceCollection services)
        {
            services.Configure<IdentityServerOptions>(options => Configuration.GetSection("ApiAuthentication").Bind(options))
                .AddTransient<ISchemeChangeSubscriber, SchemeChangeSubscriber<SchemeDefinition>>()
                .AddDbContext<ApplicationDbContext>(options => options.UseDatabaseFromConfiguration(Configuration))
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

        private void AddProxyServices(IServiceCollection services)
        {
            void configureOptions(IdentityServerOptions options)
                => Configuration.GetSection("PrivateServerAuthentication").Bind(options);

            services.AddTransient<ISchemeChangeSubscriber, SchemeChangeSubscriber<Auth.SchemeDefinition>>()
                .AddIdentityProviderStore()
                .AddConfigurationHttpStores(configureOptions)
                .AddOperationalHttpStores()
                .AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = Configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                .AddTheIdServerStores(configureOptions)
                .AddDefaultTokenProviders();
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