// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Duende.IdentityServer.Models;
using System;
using System.Globalization;
using System.Linq;

namespace Aguacongas.IdentityServer.Store
{
    public static class EntityExtensions
    {
        public static Client ToClient(this Entity.Client client)
        {
            if (client == null)
            {
                return null;
            }
            var cultureId = CultureInfo.CurrentCulture.Name;
            var resources = client.Resources;
            var policyUri = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.PolicyUri
                    && r.CultureId == cultureId)?.Value ?? client.PolicyUri;
            if (policyUri != null)
            {
                client.Properties.Add(new Entity.ClientProperty
                {
                    Key = "PolicyUrl",
                    Value = policyUri
                });
            }
            var tosUri = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.TosUri
                    && r.CultureId == cultureId)?.Value ?? client.TosUri;
            if (policyUri != null)
            {
                client.Properties.Add(new Entity.ClientProperty
                {
                    Key = "TosUrl",
                    Value = tosUri
                });
            }
            return new Client
            {
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AccessTokenType = (AccessTokenType)client.AccessTokenType,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = client.RedirectUris
                    .Where(u => (u.Kind & Entity.UriKinds.Cors) == Entity.UriKinds.Cors)
                    .Select(u => new Uri(u.Uri))
                    .Select(u => $"{u.Scheme}://{u.Host}{u.UriPortString()}")
                    .ToList(),
                AllowedGrantTypes = client.AllowedGrantTypes.Select(g => g.GrantType).ToList(),
                AllowedScopes = client.AllowedScopes.Select(s => s.Scope).ToList(),
                AllowOfflineAccess = client.AllowOfflineAccess,
                AllowPlainTextPkce = client.AllowPlainTextPkce,
                AllowRememberConsent = client.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                Claims = client.ClientClaims.Select(c => new ClientClaim(c.Type, c.Value)).ToList(),
                ClientClaimsPrefix = client.ClientClaimsPrefix,
                ClientId = client.Id,
                ClientName = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.DisplayName
                    && r.CultureId == cultureId)?.Value ?? client.ClientName,
                ClientSecrets = client.ClientSecrets.Select(s => new Secret
                {
                    Value = s.Value,
                    Expiration = s.Expiration,
                    Description = s.Description,
                    Type = s.Type
                }).ToList(),
                ClientUri = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.ClientUri
                    && r.CultureId == cultureId)?.Value ?? client.ClientUri,
                ConsentLifetime = client.ConsentLifetime,
                Description = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.Description
                    && r.CultureId == cultureId)?.Value ?? client.Description,
                DeviceCodeLifetime = client.DeviceCodeLifetime,
                Enabled = client.Enabled,
                EnableLocalLogin = client.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                IdentityProviderRestrictions = client.IdentityProviderRestrictions.Select(r => r.Provider).ToList(),
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                IncludeJwtId = client.IncludeJwtId,
                LogoUri = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.LogoUri
                    && r.CultureId == cultureId)?.Value ?? client.LogoUri,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                PostLogoutRedirectUris = client.RedirectUris
                    .Where(u => (u.Kind & Entity.UriKinds.PostLogout) == Entity.UriKinds.PostLogout)
                    .Select(u => u.Uri).ToList(),
                Properties = client.Properties.ToDictionary(p => p.Key, p => p.Value),
                ProtocolType = client.ProtocolType,
                RefreshTokenExpiration = (TokenExpiration)client.RefreshTokenExpiration,
                RedirectUris = client.RedirectUris
                    .Where(u => (u.Kind & Entity.UriKinds.Redirect) == Entity.UriKinds.Redirect)
                    .Select(u => u.Uri).ToList(),
                RefreshTokenUsage = (TokenUsage)client.RefreshTokenUsage,
                RequireClientSecret = client.RequireClientSecret,
                RequireConsent = client.RequireConsent,
                RequirePkce = client.RequirePkce,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = client.UserCodeType,
                UserSsoLifetime = client.UserSsoLifetime,
                AllowedIdentityTokenSigningAlgorithms = client.AllowedIdentityTokenSigningAlgorithms.Select(a => a.Algorithm).ToList(),
                CibaLifetime = client.CibaLifetime,
                CoordinateLifetimeWithUserSession = client.CoordinateLifetimeWithUserSession,
                PollingInterval = client.PollingInterval,
                RequireRequestObject = client.RequireRequestObject,
                RequireDPoP = client.RequireDPoP
            };
        }

        public static ApiResource ToApi(this Entity.ProtectResource api)
        {
            if (api == null)
            {
                return null;
            }
            var cultureId = CultureInfo.CurrentCulture.Name;
            var resources = api.Resources;

            return new ApiResource
            {
                ApiSecrets = api.Secrets.Select(s => new Secret { Value = s.Value, Expiration = s.Expiration }).ToList(),
                Description = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.Description
                    && r.CultureId == cultureId)?.Value ?? api.Description,
                DisplayName = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.DisplayName
                    && r.CultureId == cultureId)?.Value ?? api.DisplayName,
                Enabled = api.Enabled,
                Name = api.Id,
                Properties = api.Properties.ToDictionary(p => p.Key, p => p.Value),
                Scopes = api.ApiScopes.Select(s => s.ApiScopeId).ToList(),
                UserClaims = api.ApiClaims.Select(c => c.Type).ToList(),
                RequireResourceIndicator = api.RequireResourceIndicator
            };
        }

        public static IdentityResource ToIdentity(this Entity.IdentityResource identity)
        {
            if (identity == null)
            {
                return null;
            }
            var cultureId = CultureInfo.CurrentCulture.Name;
            var resources = identity.Resources;

            return new IdentityResource
            {
                Description = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.Description
                    && r.CultureId == cultureId)?.Value ?? identity.Description,
                DisplayName = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.DisplayName
                    && r.CultureId == cultureId)?.Value ?? identity.DisplayName,
                Emphasize = identity.Emphasize,
                Enabled = identity.Enabled,
                Name = identity.Id,
                Properties = identity.Properties.ToDictionary(p => p.Key, p => p.Value),
                Required = identity.Required,
                ShowInDiscoveryDocument = identity.ShowInDiscoveryDocument,
                UserClaims = identity.IdentityClaims.Select(c => c.Type).ToList()
            };
        }

        public static ApiScope ToApiScope(this Entity.ApiScope apiScope)
        {
            if (apiScope == null)
            {
                return null;
            }
            var cultureId = CultureInfo.CurrentCulture.Name;
            var resources = apiScope.Resources;
            return new ApiScope
            {
                Description = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.Description
                    && r.CultureId == cultureId)?.Value ?? apiScope.Description,
                DisplayName = resources.FirstOrDefault(r => r.ResourceKind == Entity.EntityResourceKind.DisplayName
                && r.CultureId == cultureId)?.Value ?? apiScope.DisplayName,
                Emphasize = apiScope.Emphasize,
                Name = apiScope.Id,
                Required = apiScope.Required,
                ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
                UserClaims = apiScope.ApiScopeClaims
                        .Select(c => c.Type).ToList(),
                Properties = apiScope.Properties
                        .ToDictionary(p => p.Key, p => p.Value)
            };
        }

        public static bool CorsMatch(this Uri cors, string url)
        {
            cors = cors ?? throw new ArgumentNullException(nameof(cors));
            url = url ?? throw new ArgumentNullException(nameof(url));
            var uri = new Uri(url);
            return uri.Scheme.ToUpperInvariant() == cors.Scheme.ToUpperInvariant() &&
                uri.Host.ToUpperInvariant() == cors.Host.ToUpperInvariant() &&
                uri.Port == cors.Port;
        }

        private static string UriPortString(this Uri uri)
        => uri.IsDefaultPort ? "" : $":{uri.Port}";
    }
}
