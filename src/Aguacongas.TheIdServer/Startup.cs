// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Options;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

            ConfigureDataProtection(services);

            var identityBuilder = services.AddClaimsProviders(Configuration)
                .Configure<ForwardedHeadersOptions>(Configuration.GetSection(nameof(ForwardedHeadersOptions)))
                .Configure<AccountOptions>(Configuration.GetSection(nameof(AccountOptions)))
                .Configure<DynamicClientRegistrationOptions>(Configuration.GetSection(nameof(DynamicClientRegistrationOptions)))
                .Configure<TokenValidationParameters>(Configuration.GetSection(nameof(TokenValidationParameters)))
                .Configure<SiteOptions>(Configuration.GetSection(nameof(SiteOptions)))
                .ConfigureNonBreakingSameSiteCookies()
                .AddOidcStateDataFormatterCache()
                .AddIdentityServer(Configuration.GetSection(nameof(IdentityServerOptions)))
                .AddAspNetIdentity<ApplicationUser>()
                .AddDynamicClientRegistration()
                .ConfigureKey(Configuration.GetSection("IdentityServer:Key"));

            identityBuilder.AddJwtRequestUriHttpClient();

            if (isProxy)
            {
                identityBuilder.Services.AddTransient<IProfileService>(p =>
                {
                    var options = p.GetRequiredService<IOptions<IdentityServerOptions>>().Value;
                    var httpClient = p.GetRequiredService<IHttpClientFactory>().CreateClient(options.HttpClientName);
                    return new ProxyProfilService<ApplicationUser>(httpClient,
                        p.GetRequiredService<UserManager<ApplicationUser>>(),
                        p.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                        p.GetRequiredService<IEnumerable<IProvideClaims>>(),
                        p.GetRequiredService<ILogger<ProxyProfilService<ApplicationUser>>>());
                });
            }
            else
            {
                identityBuilder.AddProfileService<ProfileService<ApplicationUser>>();
                if (!Configuration.GetValue<bool>("DisableTokenCleanup"))
                {
                    identityBuilder.AddTokenCleaner(Configuration.GetValue<TimeSpan?>("TokenCleanupInterval") ?? TimeSpan.FromMinutes(1));
                }
            }

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
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, ConfigureIdentityServerAuthenticationOptions());


            var mvcBuilder = services.Configure<SendGridOptions>(Configuration)
                .AddLocalization()
                .AddControllersWithViews(options =>
                    options.AddIdentityServerAdminFilters())
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
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
                .UseIdentityServer();

            if (!isProxy)
            {
                app.UseIdentityServerAdminAuthentication("/providerhub", JwtBearerDefaults.AuthenticationScheme);
            }

            app
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
        }

        private Action<IdentityServerAuthenticationOptions> ConfigureIdentityServerAuthenticationOptions()
        {
            return options =>
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
                    var path = request.Path;
                    var accessToken = TokenRetrieval.FromQueryString()(request);
                    if (path.StartsWithSegments("/providerhub") && !string.IsNullOrEmpty(accessToken))
                    {
                        return accessToken;
                    }
                    var oneTimeToken = TokenRetrieval.FromQueryString("otk")(request);
                    if (!string.IsNullOrEmpty(oneTimeToken))
                    {
                        return request.HttpContext
                            .RequestServices
                            .GetRequiredService<IRetrieveOneTimeToken>()
                            .GetOneTimeToken(oneTimeToken);
                    }
                    return TokenRetrieval.FromAuthorizationHeader()(request);
                }

                options.TokenRetriever = tokenRetriever;
            };
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
                    options => Configuration.GetSection(nameof(IdentityOptions)).Bind(options))
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
                    options => Configuration.GetSection(nameof(IdentityOptions)).Bind(options))
                .AddTheIdServerStores(configureOptions)
                .AddDefaultTokenProviders();
        }

        private void ConfigureInitialData(IApplicationBuilder app)
        {
            var dbType = Configuration.GetValue<DbTypes>("DbType");
            if (Configuration.GetValue<bool>("Migrate") &&
                dbType != DbTypes.InMemory)
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
        private void ConfigureDataProtection(IServiceCollection services)
        {
            var dataprotectionSection = Configuration.GetSection(nameof(DataProtectionOptions));
            if (dataprotectionSection != null)
            {
                services.AddDataProtection(options => dataprotectionSection.Bind(options)).ConfigureDataProtection(Configuration.GetSection(nameof(DataProtectionOptions)));
            }
        }
    }
}