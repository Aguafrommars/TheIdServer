﻿using Aguacongas.DynamicConfiguration.Redis;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Aguacongas.IdentityServer.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;
using Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;
using Aguacongas.TheIdServer.Identity.ScryptPasswordHasher;
using Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;
using Aguacongas.TheIdServer.Models;
using Aguacongas.TheIdServer.Services;
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Services;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using ConfigurationModel = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTheIdServer(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            var isProxy = configurationManager.GetValue<bool>("Proxy");
            var dbType = configurationManager.GetValue<DbTypes>("DbType");

            services.AddTransient<ISchemeChangeSubscriber, SchemeChangeSubscriber<SchemeDefinition>>()
                .AddIdentityProviderStore()
                .AddConfigurationStores()
                .AddOperationalStores()
                .AddTokenExchange()
                .Configure<PasswordHasherOptions>(configurationManager.GetSection(nameof(PasswordHasherOptions)))
                .AddIdentity<ApplicationUser, IdentityRole>(
                    options =>
                    {
                        configurationManager.Bind(nameof(AspNetCore.Identity.IdentityOptions), options);
                    })
                .AddTheIdServerStores()
                .AddDefaultTokenProviders()
                .AddArgon2PasswordHasher<ApplicationUser>(configurationManager.GetSection(nameof(Argon2PasswordHasherOptions)))
                .AddBcryptPasswordHasher<ApplicationUser>(configurationManager.GetSection(nameof(BcryptPasswordHasherOptions)))
                .AddScryptPasswordHasher<ApplicationUser>(configurationManager.GetSection(nameof(ScryptPasswordHasherOptions)))
                .AddUpgradePasswordHasher<ApplicationUser>(configurationManager.GetSection(nameof(UpgradePasswordHasherOptions)));

            if (isProxy)
            {
                services.AddAdminHttpStores(options => configurationManager.Bind("PrivateServerAuthentication", options));
            }
            else
            {
                AddDefaultServices(services, configurationManager, dbType);
            }

            ConfigureDataProtection(services, configurationManager);

            var identityServerBuilder = services.AddClaimsProviders(configurationManager)
                .Configure<ForwardedHeadersOptions>(configurationManager.GetSection(nameof(ForwardedHeadersOptions)))
                .Configure<AccountOptions>(configurationManager.GetSection(nameof(AccountOptions)))
                .Configure<Aguacongas.IdentityServer.Admin.Options.DynamicClientRegistrationOptions>(configurationManager.GetSection(nameof(Aguacongas.IdentityServer.Admin.Options.DynamicClientRegistrationOptions)))
                .Configure<TokenValidationParameters>(configurationManager.GetSection(nameof(TokenValidationParameters)))
                .Configure<SiteOptions>(configurationManager.GetSection(nameof(SiteOptions)))
                .ConfigureNonBreakingSameSiteCookies()
                .AddOidcStateDataFormatterCache()
                .Configure<IdentityServerOptions>(configurationManager.GetSection(nameof(IdentityServerOptions)))
                .Configure<DynamicProviderOptions>(options => { })
                .AddTransient(p => p.GetRequiredService<IOptions<DynamicProviderOptions>>().Value)
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
                .AddCiba(configurationManager.GetSection(nameof(BackchannelAuthenticationUserNotificationServiceOptions)))
                .AddAspNetIdentity<ApplicationUser>()
                .AddDynamicClientRegistration()
                .AddJwtBearerClientAuthentication()
                .AddMutualTlsSecretValidators()
                .ConfigureDiscovey(configurationManager.GetSection(nameof(Aguacongas.IdentityServer.IdentityServerOptions)))
                .ConfigureKey(configurationManager.GetSection("IdentityServer:Key"));

            if (configurationManager.GetValue<bool>("IdentityServerOptions:EnableServerSideSession"))
            {
                identityServerBuilder.AddServerSideSessions<ServerSideSessionStore>();
            }

            identityServerBuilder.AddJwtRequestUriHttpClient();

            if (isProxy)
            {
                identityServerBuilder.Services.AddTransient<IProfileService>(p =>
                {
                    var options = p.GetRequiredService<IOptions<Aguacongas.IdentityServer.IdentityServerOptions>>().Value;
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
                if (!configurationManager.GetValue<bool>("DisableTokenCleanup"))
                {
                    identityServerBuilder.AddTokenCleaner(configurationManager.GetValue<TimeSpan?>("TokenCleanupInterval") ?? TimeSpan.FromMinutes(1));
                }
            }

            services.AddTransient(p =>
            {
                var handler = new HttpClientHandler();
                if (configurationManager.GetValue<bool>("DisableStrictSsl"))
                {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, policy) => true;
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
                }
                return handler;
            })
                .AddHttpClient(OAuth2IntrospectionDefaults.BackChannelHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(p => p.GetRequiredService<HttpClientHandler>());

            var authenticationBuilder = services.Configure<ExternalLoginOptions>(configurationManager.GetSection("Google"))
                .AddAuthorization(options =>
                    options.AddIdentityServerPolicies())
                .AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => ConfigureIdentityServerJwtBearerOptions(options, configurationManager))
                // reference tokens
                .AddOAuth2Introspection("introspection", options => ConfigureIdentityServerOAuth2IntrospectionOptions(options, configurationManager));

            var mutulaTlsOptions = configurationManager.GetSection("IdentityServerOptions:MutualTls").Get<Aguacongas.TheIdServer.BlazorApp.Models.MutualTlsOptions>();
            if (mutulaTlsOptions?.Enabled == true)
            {
                // MutualTLS
                authenticationBuilder.AddCertificate(mutulaTlsOptions!.ClientCertificateAuthenticationScheme, 
                    options => configurationManager.Bind(nameof(CertificateAuthenticationOptions), options));
            }


            var mvcBuilder = services.Configure<SendGridOptions>(configurationManager)
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
                .AddIdentityServerWsFederation(configurationManager.GetSection(nameof(WsFederationOptions)))
                .AddIdentityServerSaml2P(configurationManager.GetSection(nameof(Saml2POptions)));

            ConfigureDynamicProviderManager(mvcBuilder, isProxy, dbType);
            ConfigureDynamicConfiguration(mvcBuilder, configurationManager);

            services.AddRemoteAuthentication<RemoteAuthenticationState, RemoteUserAccount, OidcProviderOptions>();
            services.Configure<HostModelOptions>(configurationManager.GetSection(nameof(HostModelOptions)))
                .AddScoped<ThemeService>()
                .AddScoped<LazyAssemblyLoader>()
                .AddScoped<AuthenticationStateProvider, RemoteAuthenticationService>()
                .AddScoped<NavigationManager, PreRenderNavigationManager>()
                .AddScoped<ISharedStringLocalizerAsync, Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services.StringLocalizer>()
                .AddTransient<IReadOnlyCultureStore, PreRenderCultureStore>()
                .AddTransient<IReadOnlyLocalizedResourceStore, PreRenderLocalizedResourceStore>()
                .AddTransient<IAccessTokenProvider, AccessTokenProvider>()
                .AddTransient<JSInterop.IJSRuntime, JSRuntime>()
                .AddTransient<IKeyStore<ECDsaEncryptorDescriptor>, KeyStore<ECDsaEncryptorDescriptor, Aguacongas.IdentityServer.KeysRotation.ECDsaEncryptorDescriptor>>()
                .AddTransient<IKeyStore<RsaEncryptorDescriptor>, KeyStore<RsaEncryptorDescriptor, Aguacongas.IdentityServer.KeysRotation.RsaEncryptorDescriptor>>()
                .AddTransient<IKeyStore<IAuthenticatedEncryptorDescriptor>, KeyStore<IAuthenticatedEncryptorDescriptor, ConfigurationModel.IAuthenticatedEncryptorDescriptor>>()
                .AddAdminApplication(new Settings())
                .AddDatabaseDeveloperPageExceptionFilter()
                .AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("Identity", "/Account"));

            ConfigureHealthChecks(services, dbType, isProxy, configurationManager);

            services.AddRazorComponents().AddInteractiveWebAssemblyComponents();

            return services;
        }

        private static void ConfigureDynamicConfiguration(IMvcBuilder mvcBuilder, ConfigurationManager configurationManager)
        {
            var redisOptions = new RedisConfigurationOptions();
            configurationManager.Bind(nameof(RedisConfigurationOptions), redisOptions);
            if (!string.IsNullOrEmpty(redisOptions.ConnectionString))
            {
                configurationManager.AddRedis(redisOptions);
            }

            var configurationRoot = configurationManager as IConfigurationRoot;

            var dynamicConfiguratioProviderType = configurationManager.GetValue<string>("DynamicConfigurationOptions:ProviderType");
            if (!string.IsNullOrEmpty(dynamicConfiguratioProviderType))
            {
                var providerType = Type.GetType(dynamicConfiguratioProviderType, true);
                var provider = configurationRoot.Providers.First(p => p.GetType() == providerType);
                mvcBuilder.AddConfigurationWebAPI(configurationRoot, options => options.Provider = provider);
            }
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
            configuration.Bind("ApiAuthentication", options);
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
            configuration.Bind("ApiAuthentication", options);
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
            services.Configure<Aguacongas.IdentityServer.IdentityServerOptions>(options => configuration.Bind("ApiAuthentication", options))
                .AddIdentityProviderStore();

            if (dbType == DbTypes.RavenDb)
            {
                services.Configure<RavenDbOptions>(options => configuration.Bind(nameof(RavenDbOptions), options))
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
                            documentStore.Certificate = X509CertificateLoader.LoadPkcs12FromFile(options.CertificatePath, options.CertificatePassword);
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

            var signalRBuilder = services.AddSignalR(options => configuration.Bind("SignalR:HubOptions", options));
            if (configuration.GetValue<bool>("SignalR:UseMessagePack"))
            {
                signalRBuilder.AddMessagePackProtocol();
            }

            var redisConnectionString = configuration.GetValue<string>("SignalR:RedisConnectionString");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                signalRBuilder.AddStackExchangeRedis(redisConnectionString, options => configuration.Bind("SignalR:RedisOptions", options));
            }
        }

        private static void ConfigureDataProtection(IServiceCollection services, ConfigurationManager configurationManager)
        {
            var dataProtectionSection = configurationManager.GetSection(nameof(DataProtectionOptions));
            if (dataProtectionSection != null)
            {
                services.AddDataProtection(options => dataProtectionSection.Bind(options))
                    .ConfigureDataProtection(dataProtectionSection);
            }
        }

        private static void ConfigureHealthChecks(IServiceCollection services,DbTypes dbTypes, bool isProxy, IConfiguration configuration)
        {
            var builder = services.AddHealthChecks();
            ConfigureDbHealthChecks(dbTypes, isProxy, configuration, builder);

            var dynamicConfigurationRedisConnectionString = configuration.GetValue<string>($"{nameof(RedisConfigurationOptions)}:{nameof(RedisConfigurationOptions.ConnectionString)}");
            var signalRRedisConnectionString = configuration.GetValue<string>("SignalR:RedisConnectionString");

            if (!string.IsNullOrEmpty(signalRRedisConnectionString))
            {
                builder.AddRedis(signalRRedisConnectionString, name: "signalRRedisConnectionString");
            }

            if (!string.IsNullOrEmpty(dynamicConfigurationRedisConnectionString))
            {
                builder.AddRedis(dynamicConfigurationRedisConnectionString, name: "dynamicConfigurationRedis");
            }
        }

        private static void ConfigureDbHealthChecks(DbTypes dbTypes, bool isProxy, IConfiguration configuration, IHealthChecksBuilder builder)
        {
            if (!isProxy)
            {
                var tags = new[] { "store" };
                switch (dbTypes)
                {
                    case DbTypes.MongoDb:
                        builder.AddMongoDb(tags: tags);
                        break;
                    case DbTypes.RavenDb:
                        builder.AddRavenDB(options =>
                        {
                            var section = configuration.GetSection(nameof(RavenDbOptions));
                            section.Bind(options);
                            var path = section.GetValue<string>(nameof(RavenDbOptions.CertificatePath));
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                options.Certificate = X509CertificateLoader.LoadPkcs12FromFile(path, section.GetValue<string>(nameof(RavenDbOptions.CertificatePassword)));
                            }
                        }, tags: tags);
                        break;
                    default:
                        builder.AddDbContextCheck<ConfigurationDbContext>(tags: tags)
                            .AddDbContextCheck<OperationalDbContext>(tags: tags)
                            .AddDbContextCheck<ApplicationDbContext>(tags: tags);
                        break;
                }
                return;
            }

            builder.AddAsyncCheck("api", async () =>
            {
                using var client = new HttpClient();
                var reponse = await client.GetAsync(configuration.GetValue<string>($"{nameof(PrivateServerAuthentication)}:HeathUrl")).ConfigureAwait(false);
                return new HealthCheckResult(reponse.IsSuccessStatusCode ? HealthStatus.Healthy : HealthStatus.Unhealthy);
            });
        }
    }
}
