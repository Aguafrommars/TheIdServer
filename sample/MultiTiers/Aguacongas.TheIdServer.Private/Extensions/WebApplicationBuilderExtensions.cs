// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Options;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using IdentityServerHost.Quickstart.UI;
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
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddTheIdServerPrivate(this WebApplicationBuilder webApplicationBuilder)
        {
            var configuration = webApplicationBuilder.Configuration;
            var migrationsAssembly = "Aguacongas.TheIdServer.Migrations.SqlServer";
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var services = webApplicationBuilder.Services;
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString))
                .AddTheIdServerAdminEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddConfigurationEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalEntityFrameworkStores(options =>
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddIdentityProviderStore();

            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var identityBuilder = services.AddClaimsProviders(configuration)
                .Configure<ForwardedHeadersOptions>(configuration.GetSection(nameof(ForwardedHeadersOptions)))
                .Configure<AccountOptions>(configuration.GetSection(nameof(AccountOptions)))
                .Configure<DynamicClientRegistrationOptions>(configuration.GetSection(nameof(DynamicClientRegistrationOptions)))
                .Configure<TokenValidationParameters>(configuration.GetSection(nameof(TokenValidationParameters)))
                .ConfigureNonBreakingSameSiteCookies()
                .AddOidcStateDataFormatterCache()
                .AddIdentityServer(configuration.GetSection(nameof(IdentityServerOptions)))
                .AddAspNetIdentity<ApplicationUser>()
                .AddSigningCredentials()
                .AddDynamicClientRegistration();

            identityBuilder.AddJwtRequestUriHttpClient();

            identityBuilder.AddProfileService<ProfileService<ApplicationUser>>();
            if (!configuration.GetValue<bool>("DisableTokenCleanup"))
            {
                identityBuilder.AddTokenCleaner(configuration.GetValue<TimeSpan?>("TokenCleanupInterval") ?? TimeSpan.FromMinutes(1));
            }

            services.AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, ConfigureIdentityServerAuthenticationOptions(configuration));

            services.Configure<SendGridOptions>(configuration)
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
                })
                .AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>();

            services.AddDatabaseDeveloperPageExceptionFilter()
                .AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));

            return webApplicationBuilder;
        }

        private static Action<IdentityServerAuthenticationOptions> ConfigureIdentityServerAuthenticationOptions(IConfiguration configuration)
        {
            return options =>
            {
                configuration.GetSection("ApiAuthentication").Bind(options);
                if (configuration.GetValue<bool>("DisableStrictSsl"))
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
    }
}
