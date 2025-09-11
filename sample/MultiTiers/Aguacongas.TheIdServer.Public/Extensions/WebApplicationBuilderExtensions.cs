// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Options;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer.Services;
using Duende.AspNetCore.Authentication.OAuth2Introspection;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Duende.AspNetCore.Authentication.OAuth2Introspection.Infrastructure;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddTheIdServerPublic(this WebApplicationBuilder webApplicationBuilder)
        {
            var configuration = webApplicationBuilder.Configuration;

            void configureOptions(IdentityServerOptions options)
                => configuration.GetSection("PrivateServerAuthentication").Bind(options);

            var services = webApplicationBuilder.Services;
            services.AddTransient<HttpClientHandler>();

            services.AddIdentityProviderStore()
                .AddConfigurationStores()
                .AddAdminHttpStores(configureOptions)
                .AddOperationalStores()
                .AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = configuration.GetValue<bool>("SignInOptions:RequireConfirmedAccount"))
                .AddTheIdServerStores()
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

            services.AddTransient(p =>
            {
                var handler = new HttpClientHandler();
                if (configuration.GetValue<bool>("DisableStrictSsl"))
                {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, policy) => true;
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                }
                return handler;
            })
                .AddHttpClient(OAuth2IntrospectionDefaults.BackChannelHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(p => p.GetRequiredService<HttpClientHandler>());

            services.AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, ConfigureIdentityServerAuthenticationOptions(configuration));

            var mvcBuilder = services.Configure<SendGridOptions>(configuration)
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

            mvcBuilder.AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>();

            services.AddDatabaseDeveloperPageExceptionFilter()
                .AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));

            return webApplicationBuilder;
        }

        private static Action<JwtBearerOptions> ConfigureIdentityServerAuthenticationOptions(IConfiguration configuration)
        => options =>
        {
            configuration.GetSection("ApiAuthentication").Bind(options);
            if (configuration.GetValue<bool>("DisableStrictSsl"))
            {
                options.BackchannelHttpHandler = new HttpClientHandler
                {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                    ServerCertificateCustomValidationCallback = (message, cert, chain, policy) => true
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                };
            }
            options.Audience = configuration["ApiAuthentication:ApiName"];
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var request = context.HttpContext.Request;
                    var path = request.Path;
                    var accessToken = TokenRetrieval.FromQueryString()(request);
                    if (path.StartsWithSegments("/providerhub") && !string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                    var oneTimeToken = TokenRetrieval.FromQueryString("otk")(request);
                    if (!string.IsNullOrEmpty(oneTimeToken))
                    {
                        context.Token = request.HttpContext
                            .RequestServices
                            .GetRequiredService<IRetrieveOneTimeToken>()
                            .ConsumeOneTimeToken(oneTimeToken);
                        return Task.CompletedTask;
                    }
                    context.Token = TokenRetrieval.FromAuthorizationHeader()(request);
                    return Task.CompletedTask;
                }
            };
        };
    }
}
