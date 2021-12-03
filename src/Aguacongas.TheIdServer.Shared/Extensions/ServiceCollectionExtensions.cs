using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Admin.Options;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Aguacongas.TheIdServer.Models;
using Aguacongas.TheIdServer.Services;
using Aguacongas.TheIdServer.UI;
using IdentityModel.AspNetCore.OAuth2Introspection;
#if DUENDE
using Duende.IdentityServer.Services;
#else
using IdentityServer4.Services;
#endif
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ConfigurationModel = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTheIdServer(this IServiceCollection services, IConfiguration configuration)
        {
            var isProxy = configuration.GetValue<bool>("Proxy");
            var dbType = configuration.GetValue<DbTypes>("DbType");

            services.AddTransient<ISchemeChangeSubscriber, SchemeChangeSubscriber<SchemeDefinition>>()
                .AddIdentityProviderStore()
                .AddConfigurationStores()
                .AddOperationalStores()
                .AddIdentity<ApplicationUser, IdentityRole>(
                    options => configuration.GetSection(nameof(IdentityOptions)).Bind(options))
                .AddTheIdServerStores()
                .AddDefaultTokenProviders();

            if (isProxy)
            {
                services.AddAdminHttpStores(options => configuration.GetSection("PrivateServerAuthentication").Bind(options));
            }
            else
            {
                AddDefaultServices(services, configuration, dbType);
            }

            ConfigureDataProtection(services, configuration);

            var identityServerBuilder = services.AddClaimsProviders(configuration)
                .Configure<ForwardedHeadersOptions>(configuration.GetSection(nameof(ForwardedHeadersOptions)))
                .Configure<AccountOptions>(configuration.GetSection(nameof(AccountOptions)))
                .Configure<DynamicClientRegistrationOptions>(configuration.GetSection(nameof(DynamicClientRegistrationOptions)))
                .Configure<TokenValidationParameters>(configuration.GetSection(nameof(TokenValidationParameters)))
                .Configure<SiteOptions>(configuration.GetSection(nameof(SiteOptions)))
                .ConfigureNonBreakingSameSiteCookies()
                .AddOidcStateDataFormatterCache()
#if DUENDE
                .Configure<Duende.IdentityServer.Configuration.IdentityServerOptions>(configuration.GetSection(nameof(Duende.IdentityServer.Configuration.IdentityServerOptions)))
                .AddIdentityServerBuilder()
                .AddRequiredPlatformServices()
                .AddCookieAuthentication()
                .AddCoreServices()
                .AddDefaultEndpoints()
                .AddPluggableServices()
                .AddKeyManagement()
                .AddValidators()
                .AddResponseGenerators()
                .AddDefaultSecretParsers()
                .AddDefaultSecretValidators()
                .AddInMemoryPersistedGrants()
#else
                .AddIdentityServer(configuration.GetSection(nameof(IdentityServer4.Configuration.IdentityServerOptions)))
#endif
                .AddAspNetIdentity<ApplicationUser>()
                .AddDynamicClientRegistration()
                .ConfigureKey(configuration.GetSection("IdentityServer:Key"));


            identityServerBuilder.AddJwtRequestUriHttpClient();

            if (isProxy)
            {
                identityServerBuilder.Services.AddTransient<IProfileService>(p =>
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
                identityServerBuilder.AddProfileService<ProfileService<ApplicationUser>>();
                if (!configuration.GetValue<bool>("DisableTokenCleanup"))
                {
                    identityServerBuilder.AddTokenCleaner(configuration.GetValue<TimeSpan?>("TokenCleanupInterval") ?? TimeSpan.FromMinutes(1));
                }
            }

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

            services.Configure<ExternalLoginOptions>(configuration.GetSection("Google"))
                .AddAuthorization(options =>
                    options.AddIdentityServerPolicies(true))
                .AddAuthentication()
                .AddJwtBearer("Bearer", options => ConfigureIdentityServerJwtBearerOptions(options, configuration))
                // reference tokens
                .AddOAuth2Introspection("introspection", options => ConfigureIdentityServerOAuth2IntrospectionOptions(options, configuration));

            var mvcBuilder = services.Configure<SendGridOptions>(configuration)
                .AddLocalization()
                .AddControllersWithViews(options =>
                    options.AddIdentityServerAdminFilters())
                .AddTheIdServer()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson(options =>
                {
                    var settings = options.SerializerSettings;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddIdentityServerWsFederation();

            ConfigureDynamicProviderManager(mvcBuilder, isProxy, dbType);

            services.AddRemoteAuthentication<RemoteAuthenticationState, RemoteUserAccount, OidcProviderOptions>();
            services.Configure<HostModelOptions>(configuration.GetSection(nameof(HostModelOptions)))
                 .AddScoped<LazyAssemblyLoader>()
                 .AddScoped<AuthenticationStateProvider, RemoteAuthenticationService>()
                 .AddScoped<SignOutSessionStateManager>()
                 .AddScoped<ISharedStringLocalizerAsync, Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services.StringLocalizer>()
                 .AddTransient<IReadOnlyCultureStore, PreRenderCultureStore>()
                 .AddTransient<IReadOnlyLocalizedResourceStore, PreRenderLocalizedResourceStore>()
                 .AddTransient<IAccessTokenProvider, AccessTokenProvider>()
                 .AddTransient<JSInterop.IJSRuntime, JSRuntime>()
                 .AddTransient<IKeyStore<RsaEncryptorDescriptor>, KeyStore<RsaEncryptorDescriptor, Aguacongas.IdentityServer.KeysRotation.RsaEncryptorDescriptor>>()
                 .AddTransient<IKeyStore<IAuthenticatedEncryptorDescriptor>, KeyStore<IAuthenticatedEncryptorDescriptor, ConfigurationModel.IAuthenticatedEncryptorDescriptor>>()
                 .AddAdminApplication(new Settings())
                 .AddDatabaseDeveloperPageExceptionFilter()
                 .AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));

            return services;
        }

        private static void ConfigureDynamicProviderManager(IMvcBuilder mvcBuilder, bool isProxy, DbTypes dbType)
        {
            var dynamicBuilder = mvcBuilder.AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>();
            if (isProxy)
            {
                dynamicBuilder.AddTheIdServerStore();
            }
            else if (dbType == DbTypes.MongoDb)
            {
                dynamicBuilder.AddTheIdServerEntityMongoDbStore();
            }
            else if (dbType == DbTypes.RavenDb)
            {
                dynamicBuilder.AddTheIdServerStoreRavenDbStore();
            }
            else
            {
                dynamicBuilder.AddTheIdServerEntityFrameworkStore();
            }
        }

        private static void ConfigureIdentityServerJwtBearerOptions(JwtBearerOptions options, IConfiguration configuration)
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

            options.ForwardDefaultSelector = context =>
            {
                var request = context.Request;
                var token = TokenRetrieval.FromAuthorizationHeader()(request) ?? TokenRetrieval.FromQueryString()(request);
                if (string.IsNullOrEmpty(token))
                {
                    var otk = TokenRetrieval.FromQueryString("otk")(request);
                    if (otk == null)
                    {
                        return null;
                    }
                    token = request.HttpContext
                            .RequestServices
                            .GetRequiredService<IRetrieveOneTimeToken>()
                            .GetOneTimeToken(otk);
                }

                if (token?.Contains('.') == false)
                {
                    return "introspection";
                }

                return null;
            };
        }

        private static void ConfigureIdentityServerOAuth2IntrospectionOptions(OAuth2IntrospectionOptions options, IConfiguration configuration)
        {
            configuration.GetSection("ApiAuthentication").Bind(options);
            options.ClientId = configuration.GetValue<string>("ApiAuthentication:ApiName");
            options.ClientSecret = configuration.GetValue<string>("ApiAuthentication:ApiSecret");
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
                        .ConsumeOneTimeToken(oneTimeToken);
                }
                return TokenRetrieval.FromAuthorizationHeader()(request);
            }

            options.TokenRetriever = tokenRetriever;
        }

        private static void AddDefaultServices(IServiceCollection services, IConfiguration configuration, DbTypes dbType)
        {
            services.Configure<IdentityServerOptions>(options => configuration.GetSection("ApiAuthentication").Bind(options))
                .AddIdentityProviderStore();

            if (dbType == DbTypes.RavenDb)
            {
                services.Configure<RavenDbOptions>(options => configuration.GetSection(nameof(RavenDbOptions)).Bind(options))
                    .AddSingleton(p =>
                    {
                        var options = p.GetRequiredService<IOptions<RavenDbOptions>>().Value;
                        var documentStore = new DocumentStore
                        {
                            Urls = options.Urls,
                            Database = options.Database,
                        };
                        if (!string.IsNullOrWhiteSpace(options.CertificatePath))
                        {
                            documentStore.Certificate = new X509Certificate2(options.CertificatePath, options.CertificatePassword);
                        }
                        documentStore.SetFindIdentityPropertyForIdentityServerStores();
                        return documentStore.Initialize();
                    })
                    .AddTheIdServerRavenDbStores();

            }
            else if (dbType == DbTypes.MongoDb)
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                services.AddTheIdServerMongoDbStores(connectionString);
            }
            else
            {
                services.AddTheIdServerAdminEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(configuration))
                    .AddConfigurationEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(configuration))
                    .AddOperationalEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(configuration));
            }

            var signalRBuilder = services.AddSignalR(options => configuration.GetSection("SignalR:HubOptions").Bind(options));
            if (configuration.GetValue<bool>("SignalR:UseMessagePack"))
            {
                signalRBuilder.AddMessagePackProtocol();
            }

            var redisConnectionString = configuration.GetValue<string>("SignalR:RedisConnectionString");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                signalRBuilder.AddStackExchangeRedis(redisConnectionString, options => configuration.GetSection("SignalR:RedisOptions").Bind(options));
            }
        }

        private static void ConfigureDataProtection(IServiceCollection services, IConfiguration configuration)
        {
            var dataprotectionSection = configuration.GetSection(nameof(DataProtectionOptions));
            if (dataprotectionSection != null)
            {
                services.AddDataProtection(options => dataprotectionSection.Bind(options)).ConfigureDataProtection(configuration.GetSection(nameof(DataProtectionOptions)));
            }
        }
    }
}
