// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using Entity = Aguacongas.IdentityServer.Store.Entity;
using ISModels = Duende.IdentityServer.Models;

namespace Aguacongas.TheIdServer;

/// <summary>
/// Provides methods to seed initial data for IdentityServer, including users, clients, resources, and localization.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// The JSON serializer options used for deserialization of localization files.
    /// </summary>
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Ensures the database is migrated and seeds initial data including users and configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="services">The service provider.</param>
    public static void EnsureSeedData(IConfiguration configuration, IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbType = configuration.GetValue<DbTypes>("DbType");
        if (dbType != DbTypes.InMemory && dbType != DbTypes.RavenDb && dbType != DbTypes.MongoDb)
        {
            var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            configContext.Database.Migrate();

            var opContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
            opContext.Database.Migrate();

            var appcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            appcontext.Database.Migrate();
        }

        SeedUsers(scope, configuration);
        SeedConfiguration(scope, configuration);
    }

    /// <summary>
    /// Seeds initial users and roles from configuration.
    /// </summary>
    /// <param name="scope">The service scope.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void SeedUsers(IServiceScope scope, IConfiguration configuration)
    {
        var provider = scope.ServiceProvider;
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedUsers");
        var roleMgr = provider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new string[]
        {
            SharedConstants.WRITERPOLICY,
            SharedConstants.READERPOLICY
        };
        foreach (var role in roles)
        {
            if (roleMgr.FindByNameAsync(role).GetAwaiter().GetResult() == null)
            {
                ExcuteAndCheckResult(() => roleMgr.CreateAsync(new IdentityRole
                {
                    Name = role
                })).GetAwaiter().GetResult();
            }
        }

        var userMgr = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var userList = configuration.GetSection("InitialData:Users").Get<IEnumerable<ApplicationUser>>() ?? [];
        int index = 0;
        foreach (var user in userList)
        {
            var existing = userMgr.FindByNameAsync(user.UserName!).GetAwaiter().GetResult();
            if (existing != null)
            {
                logger.LogInformation("{UserName} already exists", user.UserName);
                continue;
            }
            var pwd = configuration.GetValue<string>($"InitialData:Users:{index}:Password");
            ExcuteAndCheckResult(() => userMgr.CreateAsync(user, pwd!))
                .GetAwaiter().GetResult();

            var claimList = configuration.GetSection($"InitialData:Users:{index}:Claims").Get<IEnumerable<Entity.UserClaim>>()!
                .Select(c => new Claim(c.ClaimType, c.ClaimValue, c.OriginalType, c.Issuer))
                .ToList();
            claimList.Add(new Claim(JwtClaimTypes.UpdatedAt, DateTime.Now.ToEpochTime().ToString(), ClaimValueTypes.Integer64));
            ExcuteAndCheckResult(() => userMgr.AddClaimsAsync(user, claimList))
                .GetAwaiter().GetResult();

            var roleList = configuration.GetSection($"InitialData:Users:{index}:Roles").Get<IEnumerable<string>>();
            ExcuteAndCheckResult(() => userMgr.AddToRolesAsync(user, roleList!))
                .GetAwaiter().GetResult();

            logger.LogInformation("{UserName} created", user.UserName);

            index++;
        }
    }

    /// <summary>
    /// Seeds IdentityServer configuration including clients, resources, and localization.
    /// </summary>
    /// <param name="scope">The service scope.</param>
    /// <param name="configuration">The application configuration.</param>
    public static void SeedConfiguration(IServiceScope scope, IConfiguration configuration)
    {
        var provider = scope.ServiceProvider;
        SeedClients(configuration, provider);
        SeedIdentities(provider);
        SeedApiScopes(configuration, provider);
        SeedApis(configuration, provider);
        SeedRelyingParties(configuration, provider);
        SeedLocalization(provider);
    }

    /// <summary>
    /// Seeds localization data from JSON files.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    private static void SeedLocalization(IServiceProvider provider)
    {
        var cultureFiles = Directory.EnumerateFiles(".", "Localization-*.json");
        foreach (var file in cultureFiles)
        {
            SeedCultureFile(provider, file);
        }
    }

    /// <summary>
    /// Seeds a specific culture file into the database.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="file">The culture JSON file path.</param>
    private static void SeedCultureFile(IServiceProvider provider, string file)
    {
        var cultureName = Path.GetFileNameWithoutExtension(file).Split("-")[1];
        var cultureStore = provider.GetRequiredService<IAdminStore<Entity.Culture>>();
        var localizedResouceStore = provider.GetRequiredService<IAdminStore<Entity.LocalizedResource>>();

        var culturePage = cultureStore.GetAsync(new PageRequest
        {
            Filter = $"Id eq '{cultureName}'",
            Expand = "Resources"
        }).GetAwaiter().GetResult();
        Entity.Culture culture;
        if (culturePage.Count == 0)
        {
            culture = new Entity.Culture
            {
                Id = cultureName,
                Resources = []
            };
            try
            {
                cultureStore.CreateAsync(culture).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // key already exists
            }
        }
        else
        {
            culture = culturePage.Items.First();
        }

        var exsitings = culture.Resources.ToList();
        var resources = JsonSerializer.Deserialize<IEnumerable<Entity.LocalizedResource>>(File.ReadAllText(file), _jsonSerializerOptions);

        foreach (var resource in resources!)
        {
            if (!exsitings.Any(r => r.Key == resource.Key))
            {
                resource.CultureId = culture.Id;
                try
                {
                    localizedResouceStore.CreateAsync(resource).GetAwaiter().GetResult();
                }
                catch (ArgumentException)
                {
                    // key already exists
                }
            }
        }
    }

    /// <summary>
    /// Seeds API resources from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="provider">The service provider.</param>
    private static void SeedApis(IConfiguration configuration, IServiceProvider provider)
    {
        var apiStore = provider.GetRequiredService<IAdminStore<Entity.ProtectResource>>();
        var apiClaimStore = provider.GetRequiredService<IAdminStore<Entity.ApiClaim>>();
        var apiSecretStore = provider.GetRequiredService<IAdminStore<Entity.ApiSecret>>();
        var apiApiScopeStore = provider.GetRequiredService<IAdminStore<Entity.ApiApiScope>>();
        var apiPropertyStore = provider.GetRequiredService<IAdminStore<Entity.ApiProperty>>();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedApis");

        foreach (var resource in Config.GetApis(configuration))
        {
            if (apiStore.GetAsync(resource.Name, null).GetAwaiter().GetResult() != null)
            {
                logger.LogInformation("Api resource {Name} already exists", resource.Name);
                continue;
            }

            try
            {
                apiStore.CreateAsync(new Entity.ProtectResource
                {
                    Description = resource.Description,
                    DisplayName = resource.DisplayName,
                    Enabled = resource.Enabled,
                    Id = resource.Name,
                    RequireResourceIndicator = resource.RequireResourceIndicator
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
            SeedApiClaims(apiClaimStore, resource);
            SeedApiSecrets(apiSecretStore, resource);
            SeedApiApiScopes(apiApiScopeStore, resource);
            SeedApiProperties(apiPropertyStore, resource);

            logger.LogInformation("Add api resource {DisplayName}", resource.DisplayName);
        }
    }

    /// <summary>
    /// Seeds API properties for a given API resource.
    /// </summary>
    /// <param name="apiPropertyStore">The API property store.</param>
    /// <param name="resource">The API resource.</param>
    private static void SeedApiProperties(IAdminStore<Entity.ApiProperty> apiPropertyStore, ISModels.ApiResource resource)
    {
        foreach (var property in resource.Properties)
        {
            try
            {
                apiPropertyStore.CreateAsync(new Entity.ApiProperty
                {
                    ApiId = resource.Name,
                    Id = Guid.NewGuid().ToString(),
                    Key = property.Key,
                    Value = property.Value
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds API scopes for a given API resource.
    /// </summary>
    /// <param name="apiApiScopeStore">The API scope store.</param>
    /// <param name="resource">The API resource.</param>
    private static void SeedApiApiScopes(IAdminStore<Entity.ApiApiScope> apiApiScopeStore, ISModels.ApiResource resource)
    {
        foreach (var apiScope in resource.Scopes)
        {
            try
            {
                apiApiScopeStore.CreateAsync(new Entity.ApiApiScope
                {
                    ApiId = resource.Name,
                    ApiScopeId = apiScope,
                    Id = Guid.NewGuid().ToString()
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds API secrets for a given API resource.
    /// </summary>
    /// <param name="apiSecretStore">The API secret store.</param>
    /// <param name="resource">The API resource.</param>
    private static void SeedApiSecrets(IAdminStore<Entity.ApiSecret> apiSecretStore, ISModels.ApiResource resource)
    {
        foreach (var secret in resource.ApiSecrets)
        {
            try
            {
                apiSecretStore.CreateAsync(new Entity.ApiSecret
                {
                    ApiId = resource.Name,
                    Expiration = secret.Expiration,
                    Description = secret.Description,
                    Id = Guid.NewGuid().ToString(),
                    Type = secret.Type,
                    Value = secret.Value
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds API claims for a given API resource.
    /// </summary>
    /// <param name="apiClaimStore">The API claim store.</param>
    /// <param name="resource">The API resource.</param>
    private static void SeedApiClaims(IAdminStore<Entity.ApiClaim> apiClaimStore, ISModels.ApiResource resource)
    {
        foreach (var claim in resource.UserClaims)
        {
            try
            {
                apiClaimStore.CreateAsync(new Entity.ApiClaim
                {
                    ApiId = resource.Name,
                    Id = Guid.NewGuid().ToString(),
                    Type = claim
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds API scopes from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="provider">The service provider.</param>
    private static void SeedApiScopes(IConfiguration configuration, IServiceProvider provider)
    {
        var apiScopeStore = provider.GetRequiredService<IAdminStore<Entity.ApiScope>>();
        var apiScopeClaimStore = provider.GetRequiredService<IAdminStore<Entity.ApiScopeClaim>>();
        var apiScopePropertyStore = provider.GetRequiredService<IAdminStore<Entity.ApiScopeProperty>>();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedApiScopes");

        foreach (var resource in Config.GetApiScopes(configuration))
        {
            if (apiScopeStore.GetAsync(resource.Name, null).GetAwaiter().GetResult() != null)
            {
                logger.LogInformation("Api scope {Name} already exists", resource.Name);
                continue;
            }
            try
            {
                apiScopeStore.CreateAsync(new Entity.ApiScope
                {
                    Description = resource.Description,
                    DisplayName = resource.DisplayName,
                    Emphasize = resource.Emphasize,
                    Enabled = resource.Enabled,
                    Id = resource.Name,
                    Required = resource.Required,
                    ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }

            SeedApiScopeClaims(apiScopeClaimStore, resource);
            SeedApiScopeProperties(apiScopePropertyStore, resource);

            logger.LogInformation("Add api scope resource {DisplayName}", resource.DisplayName);
        }
    }

    /// <summary>
    /// Seeds API scope properties for a given API scope.
    /// </summary>
    /// <param name="apiScopePropertyStore">The API scope property store.</param>
    /// <param name="resource">The API scope resource.</param>
    private static void SeedApiScopeProperties(IAdminStore<Entity.ApiScopeProperty> apiScopePropertyStore, ISModels.ApiScope resource)
    {
        foreach (var property in resource.Properties)
        {
            try
            {
                apiScopePropertyStore.CreateAsync(new Entity.ApiScopeProperty
                {
                    ApiScopeId = resource.Name,
                    Id = Guid.NewGuid().ToString(),
                    Key = property.Key,
                    Value = property.Value
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds API scope claims for a given API scope.
    /// </summary>
    /// <param name="apiScopeClaimStore">The API scope claim store.</param>
    /// <param name="resource">The API scope resource.</param>
    private static void SeedApiScopeClaims(IAdminStore<Entity.ApiScopeClaim> apiScopeClaimStore, ISModels.ApiScope resource)
    {
        foreach (var claim in resource.UserClaims)
        {
            try
            {
                apiScopeClaimStore.CreateAsync(new Entity.ApiScopeClaim
                {
                    ApiScopeId = resource.Name,
                    Id = Guid.NewGuid().ToString(),
                    Type = claim
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds identity resources from configuration.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    private static void SeedIdentities(IServiceProvider provider)
    {
        var identityStore = provider.GetRequiredService<IAdminStore<Entity.IdentityResource>>();
        var identityClaimStore = provider.GetRequiredService<IAdminStore<Entity.IdentityClaim>>();
        var identityPropertyStore = provider.GetRequiredService<IAdminStore<Entity.IdentityProperty>>();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedIdentities");

        foreach (var resource in Config.GetIdentityResources())
        {
            if (identityStore.GetAsync(resource.Name, null).GetAwaiter().GetResult() != null)
            {
                logger.LogInformation("Identity resouce {Name} already exists", resource.Name);
                continue;
            }

            try
            {
                identityStore.CreateAsync(new Entity.IdentityResource
                {
                    Description = resource.Description,
                    DisplayName = resource.DisplayName,
                    Emphasize = resource.Emphasize,
                    Enabled = resource.Enabled,
                    Id = resource.Name,
                    Required = resource.Required,
                    ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
            SeedIdentityClaims(identityClaimStore, resource);
            SeedIdentityProperties(identityPropertyStore, resource);

            logger.LogInformation("Add identity resource {DisplayName}", resource.DisplayName);
        }
    }

    /// <summary>
    /// Seeds identity resource properties for a given identity resource.
    /// </summary>
    /// <param name="identityPropertyStore">The identity property store.</param>
    /// <param name="resource">The identity resource.</param>
    private static void SeedIdentityProperties(IAdminStore<Entity.IdentityProperty> identityPropertyStore, ISModels.IdentityResource resource)
    {
        foreach (var property in resource.Properties)
        {
            try
            {
                identityPropertyStore.CreateAsync(new Entity.IdentityProperty
                {
                    Id = Guid.NewGuid().ToString(),
                    IdentityId = resource.Name,
                    Key = property.Key,
                    Value = property.Value
                }).GetAwaiter().GetResult();

            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds identity resource claims for a given identity resource.
    /// </summary>
    /// <param name="identityClaimStore">The identity claim store.</param>
    /// <param name="resource">The identity resource.</param>
    private static void SeedIdentityClaims(IAdminStore<Entity.IdentityClaim> identityClaimStore, ISModels.IdentityResource resource)
    {
        foreach (var claim in resource.UserClaims)
        {
            try
            {
                identityClaimStore.CreateAsync(new Entity.IdentityClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    IdentityId = resource.Name,
                    Type = claim
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds client applications from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="provider">The service provider.</param>
    private static void SeedClients(IConfiguration configuration, IServiceProvider provider)
    {
        var clientStore = provider.GetRequiredService<IAdminStore<Entity.Client>>();
        var clientGrantTypeStore = provider.GetRequiredService<IAdminStore<Entity.ClientGrantType>>();
        var clientScopeStore = provider.GetRequiredService<IAdminStore<Entity.ClientScope>>();
        var clientClaimStore = provider.GetRequiredService<IAdminStore<Entity.ClientClaim>>();
        var clientSecretStore = provider.GetRequiredService<IAdminStore<Entity.ClientSecret>>();
        var clientIdpRestrictionStore = provider.GetRequiredService<IAdminStore<Entity.ClientIdpRestriction>>();
        var clientUriStore = provider.GetRequiredService<IAdminStore<Entity.ClientUri>>();
        var clientPropertyStore = provider.GetRequiredService<IAdminStore<Entity.ClientProperty>>();
        var clientAllowedIdentityTokenSigningAlgorithmStore = provider.GetRequiredService<IAdminStore<Entity.ClientAllowedIdentityTokenSigningAlgorithm>>();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedClients");

        foreach (var client in Config.GetClients(configuration))
        {
            if (clientStore.GetAsync(client.ClientId, null).GetAwaiter().GetResult() != null)
            {
                logger.LogInformation("Client {ClientId} already exists", client.ClientId);
                continue;
            }

            try
            {
                clientStore.CreateAsync(new Entity.Client
                {
                    AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                    AccessTokenLifetime = client.AccessTokenLifetime,
                    AccessTokenType = (int)client.AccessTokenType,
                    AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                    AllowOfflineAccess = client.AllowOfflineAccess,
                    AllowPlainTextPkce = client.AllowPlainTextPkce,
                    AllowRememberConsent = client.AllowRememberConsent,
                    AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                    AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                    AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                    BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                    BackChannelLogoutUri = client.BackChannelLogoutUri,
                    ClientClaimsPrefix = client.ClientClaimsPrefix,
                    ClientName = client.ClientName,
                    ClientUri = client.ClientUri,
                    ConsentLifetime = client.ConsentLifetime,
                    Description = client.Description,
                    DeviceCodeLifetime = client.DeviceCodeLifetime,
                    Enabled = client.Enabled,
                    EnableLocalLogin = client.EnableLocalLogin,
                    FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                    FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                    Id = client.ClientId,
                    IdentityTokenLifetime = client.IdentityTokenLifetime,
                    IncludeJwtId = client.IncludeJwtId,
                    LogoUri = client.LogoUri,
                    PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                    ProtocolType = client.ProtocolType,
                    RefreshTokenExpiration = (int)client.RefreshTokenExpiration,
                    RefreshTokenUsage = (int)client.RefreshTokenUsage,
                    RequireClientSecret = client.RequireClientSecret,
                    RequireConsent = client.RequireConsent,
                    RequirePkce = client.RequirePkce,
                    SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                    UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                    UserCodeType = client.UserCodeType,
                    UserSsoLifetime = client.UserSsoLifetime,
                    CibaLifetime = client.CibaLifetime,
                    CoordinateLifetimeWithUserSession = client.CoordinateLifetimeWithUserSession,
                    PollingInterval = client.PollingInterval,
                    RequireRequestObject = client.RequireRequestObject,
                    RequireDPoP = client.RequireDPoP,
                    PushedAuthorizationLifetime = client.PushedAuthorizationLifetime,
                    RequirePushedAuthorization = client.RequirePushedAuthorization
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
            SeedClientGrantType(clientGrantTypeStore, client);
            SeedClientScopes(clientScopeStore, client);
            SeedClientClaims(clientClaimStore, client);
            SeedClientSecrets(clientSecretStore, client);
            SeedClientRestrictions(clientIdpRestrictionStore, client);
            SeedClientProperties(clientPropertyStore, client);
            SeedClientUris(clientUriStore, client);
            SeedClientAllowedIdentityTokenSigningAlgorithms(clientAllowedIdentityTokenSigningAlgorithmStore, client);

            logger.LogInformation("Add client {ClientName}", client.ClientName);
        }
    }

    /// <summary>
    /// Seeds allowed identity token signing algorithms for a client.
    /// </summary>
    /// <param name="clientAllowedIdentityTokenSigningAlgorithmStore">The store for allowed identity token signing algorithms.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientAllowedIdentityTokenSigningAlgorithms(IAdminStore<Entity.ClientAllowedIdentityTokenSigningAlgorithm> clientAllowedIdentityTokenSigningAlgorithmStore, ISModels.Client client)
    {
        foreach (var algorythm in client.AllowedIdentityTokenSigningAlgorithms.Where(a => IdentityServerConstants.SupportedSigningAlgorithms.Contains(a)))
        {
            try
            {
                clientAllowedIdentityTokenSigningAlgorithmStore.CreateAsync(new Entity.ClientAllowedIdentityTokenSigningAlgorithm
                {
                    ClientId = client.ClientId,
                    Id = Guid.NewGuid().ToString(),
                    Algorithm = algorythm
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds client URIs including redirect, CORS, and post-logout URIs.
    /// </summary>
    /// <param name="clientUriStore">The client URI store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientUris(IAdminStore<Entity.ClientUri> clientUriStore, ISModels.Client client)
    {
        var uris = client.RedirectUris.Select(o => new Entity.ClientUri
        {
            Id = Guid.NewGuid().ToString(),
            ClientId = client.ClientId,
            Uri = o
        }).ToList();

        foreach (var origin in client.AllowedCorsOrigins)
        {
            var cors = new Uri(origin);
            var uri = uris.FirstOrDefault(u => cors.CorsMatch(u.Uri));
            var corsUri = new Uri(origin);
            var sanetized = $"{corsUri.Scheme.ToUpperInvariant()}://{corsUri.Host.ToUpperInvariant()}:{corsUri.Port}";

            if (uri == null)
            {

                uris.Add(new Entity.ClientUri
                {
                    Id = Guid.NewGuid().ToString(),
                    ClientId = client.ClientId,
                    Uri = origin,
                    Kind = Entity.UriKinds.Cors,
                    SanetizedCorsUri = sanetized
                });
                continue;
            }

            uri.SanetizedCorsUri = sanetized;
            uri.Kind = Entity.UriKinds.Redirect | Entity.UriKinds.Cors;
        }

        foreach (var postLogout in client.PostLogoutRedirectUris)
        {
            var uri = uris.FirstOrDefault(u => u.Uri == postLogout);
            if (uri == null)
            {
                uris.Add(new Entity.ClientUri
                {
                    Id = Guid.NewGuid().ToString(),
                    ClientId = client.ClientId,
                    Uri = postLogout,
                    Kind = Entity.UriKinds.PostLogout
                });
                continue;
            }

            uri.Kind |= Entity.UriKinds.Redirect;
        }

        foreach (var uri in uris)
        {
            try
            {
                clientUriStore.CreateAsync(uri).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds client properties for a client.
    /// </summary>
    /// <param name="clientPropertyStore">The client property store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientProperties(IAdminStore<Entity.ClientProperty> clientPropertyStore, ISModels.Client client)
    {
        foreach (var property in client.Properties)
        {
            try
            {
                clientPropertyStore.CreateAsync(new Entity.ClientProperty
                {
                    ClientId = client.ClientId,
                    Id = Guid.NewGuid().ToString(),
                    Key = property.Key,
                    Value = property.Value
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds client identity provider restrictions for a client.
    /// </summary>
    /// <param name="clientIdpRestrictionStore">The client IDP restriction store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientRestrictions(IAdminStore<Entity.ClientIdpRestriction> clientIdpRestrictionStore, ISModels.Client client)
    {
        foreach (var restriction in client.IdentityProviderRestrictions)
        {
            try
            {
                clientIdpRestrictionStore.CreateAsync(new Entity.ClientIdpRestriction
                {
                    ClientId = client.ClientId,
                    Id = Guid.NewGuid().ToString(),
                    Provider = restriction
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds client secrets for a client.
    /// </summary>
    /// <param name="clientSecretStore">The client secret store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientSecrets(IAdminStore<Entity.ClientSecret> clientSecretStore, ISModels.Client client)
    {
        foreach (var secret in client.ClientSecrets)
        {
            try
            {
                clientSecretStore.CreateAsync(new Entity.ClientSecret
                {
                    ClientId = client.ClientId,
                    Description = secret.Description,
                    Expiration = secret.Expiration,
                    Id = Guid.NewGuid().ToString(),
                    Type = secret.Type,
                    Value = secret.Value
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds client claims for a client.
    /// </summary>
    /// <param name="clientClaimStore">The client claim store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientClaims(IAdminStore<Entity.ClientClaim> clientClaimStore, ISModels.Client client)
    {
        foreach (var claim in client.Claims)
        {
            try
            {
                clientClaimStore.CreateAsync(new Entity.ClientClaim
                {
                    ClientId = client.ClientId,
                    Id = Guid.NewGuid().ToString(),
                    Type = claim.Type,
                    Value = claim.Value
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds allowed scopes for a client.
    /// </summary>
    /// <param name="clientScopeStore">The client scope store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientScopes(IAdminStore<Entity.ClientScope> clientScopeStore, ISModels.Client client)
    {
        foreach (var clientScope in client.AllowedScopes)
        {
            try
            {
                clientScopeStore.CreateAsync(new Entity.ClientScope
                {
                    ClientId = client.ClientId,
                    Scope = clientScope,
                    Id = Guid.NewGuid().ToString()
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds allowed grant types for a client.
    /// </summary>
    /// <param name="clientGrantTypeStore">The client grant type store.</param>
    /// <param name="client">The client model.</param>
    private static void SeedClientGrantType(IAdminStore<Entity.ClientGrantType> clientGrantTypeStore, ISModels.Client client)
    {
        foreach (var grantType in client.AllowedGrantTypes)
        {
            try
            {
                clientGrantTypeStore.CreateAsync(new Entity.ClientGrantType
                {
                    ClientId = client.ClientId,
                    GrantType = grantType,
                    Id = Guid.NewGuid().ToString()
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Seeds relying parties from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="provider">The service provider.</param>
    private static void SeedRelyingParties(IConfiguration configuration, IServiceProvider provider)
    {
        var relyingPartyStore = provider.GetRequiredService<IAdminStore<Entity.RelyingParty>>();
        var relyingPartyClaimMappingStore = provider.GetRequiredService<IAdminStore<Entity.RelyingPartyClaimMapping>>();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("SeedRelyingParties");

        foreach (var relyingParty in Config.GetRelyingParties(configuration))
        {
            if (relyingPartyStore.GetAsync(relyingParty.Id, null).GetAwaiter().GetResult() != null)
            {
                logger.LogInformation("RelyingParty {Id} already exists", relyingParty.Id);
                continue;
            }

            try
            {
                relyingPartyStore.CreateAsync(new Entity.RelyingParty
                {
                    Id = relyingParty.Id,
                    Description = relyingParty.Description,
                    DigestAlgorithm = relyingParty.DigestAlgorithm,
                    EncryptionCertificate = relyingParty.EncryptionCertificate,
                    SamlNameIdentifierFormat = relyingParty.SamlNameIdentifierFormat,
                    SignatureAlgorithm = relyingParty.SignatureAlgorithm,
                    TokenType = relyingParty.TokenType
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
            SeedRelyingPartyClaimMappings(relyingPartyClaimMappingStore, relyingParty);

            logger.LogInformation("Add RelyingParty {Id}", relyingParty.Id);
        }
    }

    /// <summary>
    /// Seeds claim mappings for a relying party.
    /// </summary>
    /// <param name="relyingPartyClaimMappingStore">The relying party claim mapping store.</param>
    /// <param name="relyingParty">The relying party entity.</param>
    private static void SeedRelyingPartyClaimMappings(IAdminStore<Entity.RelyingPartyClaimMapping> relyingPartyClaimMappingStore, Entity.RelyingParty relyingParty)
    {
        if (relyingParty.ClaimMappings == null)
        {
            return;
        }

        foreach (var mapping in relyingParty.ClaimMappings)
        {
            try
            {
                relyingPartyClaimMappingStore.CreateAsync(new Entity.RelyingPartyClaimMapping
                {
                    FromClaimType = mapping.FromClaimType,
                    Id = Guid.NewGuid().ToString(),
                    RelyingPartyId = relyingParty.Id,
                    ToClaimType = mapping.ToClaimType
                }).GetAwaiter().GetResult();
            }
            catch (ArgumentException)
            {
                // silent
            }
        }
    }

    /// <summary>
    /// Executes an asynchronous identity operation and checks the result for errors.
    /// </summary>
    /// <param name="action">The asynchronous identity operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation fails for reasons other than duplicate user name.</exception>
    private static async Task ExcuteAndCheckResult(Func<Task<IdentityResult>> action)
    {
        var result = await action.Invoke();
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            if (error.Code != "DuplicateUserName")
            {
                throw new InvalidOperationException($"{error.Description} code: {error.Code}");
            }
        }
    }
}
