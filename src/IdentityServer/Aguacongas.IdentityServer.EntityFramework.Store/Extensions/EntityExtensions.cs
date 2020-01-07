using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;

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

            var redirectUris = client.RedirectUris
                    .Where(u => (u.Kind & (int)Entity.UriKinds.Redirect) == (int)Entity.UriKinds.Redirect);

            return new Client
            {
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AccessTokenType = (AccessTokenType)client.AccessTokenType,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = client.RedirectUris
                    .Where(u => (u.Kind & (int)Entity.UriKinds.Cors) == (int)Entity.UriKinds.Cors)
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
                PostLogoutRedirectUris = redirectUris
                    .Select(u => u.Uri)
                    .ToList(),
                Properties = client.Properties.ToDictionary(p => p.Key, p => p.Value),
                ProtocolType = client.ProtocolType,
                RefreshTokenExpiration = (TokenExpiration)client.RefreshTokenExpiration,
                RedirectUris = client.RedirectUris
                    .Where(u => (u.Kind & (int)Entity.UriKinds.Redirect) == (int)Entity.UriKinds.Redirect)
                    .Select(u => u.Uri).ToList(),
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

        public static ApiResource ToApi(this Entity.ProtectResource api)
        {
            if (api == null)
            {
                return null;
            }

            return new ApiResource
            {
                ApiSecrets = api.Secrets.Select(s => new Secret { Value = s.Value, Expiration = s.Expiration }).ToList(),
                Description = api.Description,
                DisplayName = api.DisplayName,
                Enabled = api.Enabled,
                Name = api.Id,
                Properties = api.Properties.ToDictionary(p => p.Key, p => p.Value),
                Scopes = api.Scopes.Select(s => new Scope 
                { 
                    Description = s.Description, 
                    DisplayName = s.DisplayName, 
                    Emphasize = s.Emphasize, 
                    Name = s.Scope, 
                    Required = s.Required, 
                    ShowInDiscoveryDocument = s.ShowInDiscoveryDocument, 
                    UserClaims = api.ApiScopeClaims
                        .Where(s => s.ApiScpopeId == s.Id)
                        .Select(c => c.Type).ToList() 
                }).ToList(),
                UserClaims = api.ApiClaims.Select(c => c.Type).ToList()
            };
        }

        public static IdentityResource ToIdentity(this Entity.IdentityResource identity)
        {
            if (identity == null)
            {
                return null;
            }

            return new IdentityResource
            {
                Description = identity.Description,
                DisplayName = identity.DisplayName,
                Emphasize = identity.Emphasize,
                Enabled = identity.Enabled,
                Name = identity.Id,
                Properties = identity.Properties.ToDictionary(p => p.Key, p => p.Value),
                Required = identity.Required,
                ShowInDiscoveryDocument = identity.ShowInDiscoveryDocument,
                UserClaims = identity.IdentityClaims.Select(c => c.Type).ToList()
            };
        }

        public static Entity.Client ToEntity(this Client client)
        {
            if(client == null)
            {
                return null;
            }

            var uris = client.RedirectUris.Select(o => new Entity.ClientUri
            {
                Id = Guid.NewGuid().ToString(),
                Uri = o,
                Kind = (int)Entity.UriKinds.Redirect
            }).ToList();

            foreach (var origin in client.AllowedCorsOrigins)
            {
                var cors = new Uri(origin);
                var uri = uris.FirstOrDefault(u => cors.CorsMatch(u.Uri));
                if (uri == null)
                {
                    uris.Add(new Entity.ClientUri
                    {
                        Id = Guid.NewGuid().ToString(),
                        Uri = origin,
                        Kind = (int)Entity.UriKinds.Cors
                    });
                    continue;
                }

                uri.Kind |= (int)Entity.UriKinds.Cors;
            }

            foreach (var postLogout in client.PostLogoutRedirectUris)
            {
                var uri = uris.FirstOrDefault(u => u.Uri == postLogout);
                if (uri == null)
                {
                    uris.Add(new Entity.ClientUri
                    {
                        Id = Guid.NewGuid().ToString(),
                        Uri = postLogout,
                        Kind = (int)Entity.UriKinds.PostLogout
                    });
                    continue;
                }

                uri.Kind |= (int)Entity.UriKinds.Redirect;
            }

            return new Entity.Client
            {
                Id = client.ClientId,                
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AccessTokenType = (int)client.AccessTokenType,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                AllowedGrantTypes = client.AllowedGrantTypes.Select(t => new Entity.ClientGrantType
                {
                    Id = Guid.NewGuid().ToString(),
                    GrantType = t
                }).ToList(),
                AllowedScopes = client.AllowedScopes.Select(s => new Entity.ClientScope
                {
                    Id = Guid.NewGuid().ToString(),
                    Scope = s
                }).ToList(),
                AllowOfflineAccess = client.AllowOfflineAccess,
                AllowPlainTextPkce = client.AllowPlainTextPkce,
                AllowRememberConsent = client.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                ClientClaims = client.Claims.Select(c => new Entity.ClientClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = c.Type,
                    Value = c.Value
                }).ToList(),
                ClientClaimsPrefix = client.ClientClaimsPrefix,
                ClientName = client.ClientName,
                ClientSecrets = client.ClientSecrets.Select(s => new Entity.ClientSecret
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = s.Description,
                    Expiration = s.Expiration,
                    Type = s.Type,
                    Value = s.Value
                }).ToList(),
                ClientUri = client.ClientUri,
                ConsentLifetime = client.ConsentLifetime,
                Description = client.Description,
                DeviceCodeLifetime = client.DeviceCodeLifetime,
                Enabled = client.Enabled,
                EnableLocalLogin = client.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                IdentityProviderRestrictions = client.IdentityProviderRestrictions.Select(r => new Entity.ClientIdpRestriction
                {
                    Id = Guid.NewGuid().ToString(),
                    Provider = r
                }).ToList(),
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                IncludeJwtId = client.IncludeJwtId,
                LogoUri = client.LogoUri,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                Properties = client.Properties.Select(p => new Entity.ClientProperty
                {
                    Id = Guid.NewGuid().ToString(),
                    Key = p.Key,
                    Value = p.Value
                }).ToList(),
                ProtocolType = client.ProtocolType,
                RedirectUris = uris,
                RefreshTokenExpiration = (int)client.RefreshTokenExpiration,
                RefreshTokenUsage = (int)client.RefreshTokenUsage,
                RequireClientSecret = client.RequireClientSecret,
                RequireConsent = client.RequireConsent,
                RequirePkce = client.RequirePkce,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = client.UserCodeType,
                UserSsoLifetime = client.UserSsoLifetime
            };
        }

        public static Entity.ProtectResource ToEntity(this ApiResource api)
        {
            if (api == null)
            {
                return null;
            }

            return new Entity.ProtectResource
            {
                Id = api.Name,
                ApiClaims = api.UserClaims.Select(c => new Entity.ApiClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = c
                }).ToList(),
                Description = api.Description,
                DisplayName = api.DisplayName,
                Enabled = api.Enabled,
                Properties = api.Properties.Select(p => new Entity.ApiProperty
                {
                    Id = Guid.NewGuid().ToString(),
                    Key = p.Key,
                    Value = p.Value
                }).ToList(),
                Scopes = api.Scopes.Select(s => new Entity.ApiScope
                {
                    Id = Guid.NewGuid().ToString(),
                    Scope = s.Name,
                    ApiScopeClaims = s.UserClaims.Select(c => new Entity.ApiScopeClaim
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = c
                    }).ToList(),
                    Description = s.Description,
                    DisplayName = s.DisplayName,
                    Emphasize = s.Emphasize,
                    Required = s.Required,
                    ShowInDiscoveryDocument = s.ShowInDiscoveryDocument
                }).ToList(),
                Secrets = api.ApiSecrets.Select(s => new Entity.ApiSecret
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = s.Description,
                    Expiration = s.Expiration,
                    Type = s.Type,
                    Value = s.Value
                }).ToList()                
            };
        }

        public static Entity.IdentityResource ToEntity(this IdentityResource identity)
        {
            if (identity == null)
            {
                return null;
            }

            return new Entity.IdentityResource
            {
                Id = identity.Name,
                Description = identity.Description,
                DisplayName = identity.DisplayName,
                Emphasize = identity.Emphasize,
                Enabled = identity.Enabled,
                IdentityClaims = identity.UserClaims.Select(c => new Entity.IdentityClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = c
                }).ToList(),
                Properties = identity.Properties.Select(p => new Entity.IdentityProperty
                {
                    Id = Guid.NewGuid().ToString(),
                    Key = p.Key,
                    Value = p.Value
                }).ToList(),
                Required = identity.Required,
                ShowInDiscoveryDocument = identity.ShowInDiscoveryDocument
            };
        }

        public static Entity.RoleClaim ToEntity(this IdentityRoleClaim<string> claim)
        {
            return new Entity.RoleClaim
            {
                Id = claim.Id.ToString(),
                RoleId = claim.RoleId,
                Type = claim.ClaimType,
                Value = claim.ClaimValue
            };
        }

        public static Entity.UserClaim ToEntity(this IdentityUserClaim<string> claim)
        {
            return new Entity.UserClaim
            {
                Id = claim.Id.ToString(),
                UserId = claim.UserId,
                Type = claim.ClaimType,
                Value = claim.ClaimValue
            };
        }

        public static Entity.UserRole ToEntity(this IdentityUserRole<string> role)
        {
            return new Entity.UserRole
            {
                Id = $"{role.UserId}@{role.RoleId}",
                RoleId = role.RoleId,
                UserId = role.UserId
            };
        }

        public static Entity.UserToken ToEntity(this IdentityUserToken<string> token)
        {
            return new Entity.UserToken
            {
                Id = $"{token.UserId}@{token.LoginProvider}@{token.Name}",
                UserId = token.UserId,
                LoginProvider = token.LoginProvider,
                Name = token.Name,
                Value = token.Value
            };
        }

        public static Entity.UserLogin ToEntity(this IdentityUserLogin<string> login)
        {
            return new Entity.UserLogin
            {
                Id = $"{login.UserId}@{login.LoginProvider}@{login.ProviderKey}",
                UserId = login.UserId,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        public static Entity.Role ToEntity<TRole>(this TRole role) where TRole : IdentityRole
        {
            if (role == null)
            {
                return null;
            }
            return new Entity.Role
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public static TUser ToUser<TUser>(this Entity.User entity) where TUser : IdentityUser, new()
        {
            return new TUser
            {
                Id = entity.Id,
                AccessFailedCount = entity.AccessFailedCount,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                LockoutEnabled = entity.LockoutEnabled,
                LockoutEnd = entity.LockoutEnd,
                PhoneNumber = entity.PhoneNumber,
                PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                UserName = entity.UserName
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

        public static bool CorsMatch(this Uri cors, Uri uri)
        {
            cors = cors ?? throw new ArgumentNullException(nameof(cors));
            uri = uri ?? throw new ArgumentNullException(nameof(uri));

            return uri.Scheme.ToUpperInvariant() == cors.Scheme.ToUpperInvariant() &&
                uri.Host.ToUpperInvariant() == cors.Host.ToUpperInvariant() &&
                uri.Port == cors.Port;
        }

        public static TRole ToRole<TRole>(this Entity.Role entity) where TRole : IdentityRole, new()
        {
            return new TRole
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static IdentityRoleClaim<string> ToRoleClaim(this Entity.RoleClaim claim)
        {
            return new IdentityRoleClaim<string>
            {
                Id = int.Parse(claim.Id),
                RoleId = claim.RoleId,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }

        public static IdentityUserClaim<string> ToUserClaim(this Entity.UserClaim claim)
        {
            return new IdentityUserClaim<string>
            {
                Id = claim.Id != null ? int.Parse(claim.Id) : 0,
                UserId = claim.UserId,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }

        public static UserLoginInfo ToUserLoginInfo(this Entity.UserLogin login)
        {
            return new UserLoginInfo(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);
        }

        public static Entity.User ToUserEntity<TUser>(this TUser user) where TUser : IdentityUser
        {
            if (user == null)
            {
                return null;
            }
            return new Entity.User
            {
                Id = user.Id,
                AccessFailedCount = user.AccessFailedCount,
                Email = user.Email,
                UserName = user.UserName,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd.HasValue ? user.LockoutEnd.Value.DateTime : (DateTime?)null
            };
        }

        private static string UriPortString(this Uri uri)
        {
            return uri.IsDefaultPort ? "" : $":{uri.Port}";
        }
    }
}
