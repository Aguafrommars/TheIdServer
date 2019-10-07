using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

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

            return new Client
            {
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AccessTokenType = (AccessTokenType)client.AccessTokenType,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = client.AllowedCorsOrigins.Select(c => c.Origin).ToList(),
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
                Claims = client.ClientClaims.Select(c => new Claim(c.Type, c.Value)).ToList(),
                ClientClaimsPrefix = client.ClientClaimsPrefix,
                ClientId = client.Id,
                ClientName = client.ClientName,
                ClientSecrets = client.ClientSecrets.Select(s => new Secret(s.Value, s.Expiration)).ToList(),
                ClientUri = client.ClientUri,
                ConsentLifetime = client.ConsentLifetime,
                Description = client.Description,
                DeviceCodeLifetime = client.DeviceCodeLifetime,
                Enabled = client.Enabled,
                EnableLocalLogin = client.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                IdentityProviderRestrictions = client.IdentityProviderRestrictions.Select(r => r.Provider).ToList(),
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                IncludeJwtId = client.IncludeJwtId,
                LogoUri = client.LogoUri,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(u => u.Uri).ToList(),
                Properties = client.Properties.ToDictionary(p => p.Key, p => p.Value),
                ProtocolType = client.ProtocolType,
                RefreshTokenExpiration = (TokenExpiration)client.RefreshTokenExpiration,
                RedirectUris = client.RedirectUris.Select(u => u.Uri).ToList(),
                RefreshTokenUsage = (TokenUsage) client.RefreshTokenUsage,
                RequireClientSecret = client.RequireClientSecret,
                RequireConsent = client.RequireConsent,
                RequirePkce = client.RequirePkce,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = client.UserCodeType,
                UserSsoLifetime = client.UserSsoLifetime
            };
        }
    }
}
