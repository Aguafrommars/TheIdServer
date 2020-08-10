using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.ResponseHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IRegisterClientService" />
    public class RegisterClientService : IRegisterClientService
    {
        private readonly IdentityServer4.Configuration.IdentityServerOptions _options;
        private readonly IAdminStore<Client> _store;
        private readonly IDiscoveryResponseGenerator _discoveryResponseGenerator;
        private readonly IdentityServer4.Models.Client _defaultValues = new IdentityServer4.Models.Client();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterClientService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="discoveryResponseGenerator">The discovery response generator.</param>
        /// <param name="options">The identity server options.</param>
        /// <exception cref="ArgumentNullException">store</exception>
        public RegisterClientService(IAdminStore<Client> store, IDiscoveryResponseGenerator discoveryResponseGenerator, IdentityServer4.Configuration.IdentityServerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _discoveryResponseGenerator = discoveryResponseGenerator ?? throw new ArgumentNullException(nameof(discoveryResponseGenerator));
        }

        /// <summary>
        /// Registers the asynchronous.
        /// </summary>
        /// <param name="registration">The client registration.</param>
        /// <param name="uri">Base uri.</param>
        /// <returns></returns>
        public async Task<ClientRegisteration> RegisterAsync(ClientRegisteration registration, string uri)
        {
            Validate(registration);

            var secret = registration.ApplicationType == "native" ? Guid.NewGuid().ToString() : null;
            var clientName = registration.ClientNames?.FirstOrDefault(n => n.Culture == null)?.Value ?? 
                    registration.ClientNames?.FirstOrDefault()?.Value ?? Guid.NewGuid().ToString();
            var existing = await _store.GetAsync(clientName, null).ConfigureAwait(false);
            registration.Id = existing != null ? Guid.NewGuid().ToString() : clientName;           

            var client = new Client
            {
                AllowedGrantTypes = registration.GrantTypes.Select(grant => new ClientGrantType
                {
                    Id = Guid.NewGuid().ToString(),
                    GrantType = grant
                }).ToList(),
                AccessTokenLifetime = _defaultValues.AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = _defaultValues.AbsoluteRefreshTokenLifetime,
                AccessTokenType = (int)_defaultValues.AccessTokenType,
                AllowOfflineAccess = registration.ApplicationType == "web" && registration.GrantTypes.Contains("refresh_token"),
                AllowAccessTokensViaBrowser = registration.ApplicationType == "web",
                AllowedScopes = new List<ClientScope>{
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        Scope = "openid"
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        Scope = "profile"
                    }
                },
                AllowRememberConsent = _defaultValues.AllowRememberConsent,
                AllowPlainTextPkce = _defaultValues.AllowPlainTextPkce,
                AlwaysIncludeUserClaimsInIdToken = _defaultValues.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = _defaultValues.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = _defaultValues.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = _defaultValues.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = _defaultValues.BackChannelLogoutUri,
                ClientClaimsPrefix = _defaultValues.ClientClaimsPrefix,
                ClientName = clientName,
                ClientSecrets = registration.ApplicationType == "native" ? new List<ClientSecret>
                {
                    new ClientSecret
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "SharedSecret",
                        Value = IdentityServer4.Models.HashExtensions.Sha256(secret)
                    }
                } : new List<ClientSecret>(0),
                ClientUri = registration.ClientUris?.FirstOrDefault(u => u.Culture == null)?.Value ?? registration.ClientUris?.FirstOrDefault()?.Value,
                ConsentLifetime = _defaultValues.ConsentLifetime,
                Description = _defaultValues.Description,
                DeviceCodeLifetime = _defaultValues.DeviceCodeLifetime,
                Enabled = true,
                EnableLocalLogin = _defaultValues.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = _defaultValues.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = _defaultValues.FrontChannelLogoutUri,
                Id = registration.Id,
                IdentityTokenLifetime = _defaultValues.IdentityTokenLifetime,
                IncludeJwtId = _defaultValues.IncludeJwtId,
                LogoUri = registration.LogoUris?.FirstOrDefault(u => u.Culture == null)?.Value ?? registration.LogoUris?.FirstOrDefault()?.Value,
                NonEditable = false,
                PairWiseSubjectSalt = _defaultValues.PairWiseSubjectSalt,
                ProtocolType = _defaultValues.ProtocolType,
                RedirectUris = registration.RedirectUris.Select(u => new ClientUri
                {
                    Id = Guid.NewGuid().ToString(),
                    Kind = UriKinds.Cors | UriKinds.Redirect,
                    Uri = u
                }).ToList(),
                RefreshTokenExpiration = (int)_defaultValues.RefreshTokenExpiration,
                RefreshTokenUsage = (int)_defaultValues.RefreshTokenUsage,
                RequireClientSecret = secret != null,
                RequireConsent = _defaultValues.RequireConsent,
                RequirePkce = _defaultValues.RequirePkce,
                Resources = registration.ClientNames.Where(n => n.Culture != null)
                    .Select(n => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = n.Culture,
                        ResourceKind = EntityResourceKind.DisplayName,
                        Value = n.Value
                    })
                    .Union(registration.ClientUris != null ? registration.ClientUris.Where(u => u.Culture != null).Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.ClientUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .Union(registration.LogoUris != null ? registration.LogoUris?.Where(u => u.Culture != null).Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.LogoUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .Union(registration.PolicyUris != null ? registration.PolicyUris.Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.PolicyUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .Union(registration.TosUris != null ? registration.TosUris.Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.TosUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .ToList(),
                Properties = registration.Contacts != null ? new List<ClientProperty>
                {
                    new ClientProperty
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = "contacts",
                        Value = string.Join("; ", registration.Contacts)
                    }
                } : null,
                SlidingRefreshTokenLifetime = _defaultValues.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = _defaultValues.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = _defaultValues.UserCodeType,
                UserSsoLifetime = _defaultValues.UserSsoLifetime                
            };

            await _store.CreateAsync(client).ConfigureAwait(false);

            var discovery = await _discoveryResponseGenerator.CreateDiscoveryDocumentAsync(uri, uri).ConfigureAwait(false);
            registration.JwksUri = discovery["jwks_uri"].ToString();
            return registration;
        }

        private void Validate(ClientRegisteration registration)
        {
            registration.GrantTypes ??= new List<string>
            {
                "authorization_code"
            };

            if (registration.RedirectUris == null || !registration.RedirectUris.Any())
            {
                throw new RegistrationException("invalid_redirect_uri", "RedirectUri is required.");
            }

            var validationOptions = _options.Validation;
            var redirectUriList = new List<Uri>(registration.RedirectUris.Count());
            foreach(var uri in registration.RedirectUris)
            {
                if (validationOptions.InvalidRedirectUriPrefixes
                           .Any(scheme => uri?.StartsWith(scheme, StringComparison.OrdinalIgnoreCase) == true))
                {
                    throw new RegistrationException("invalid_redirect_uri", $"RedirectUri '{uri}' uses invalid scheme. If this scheme should be allowed, then configure it via ValidationOptions.");
                }
                if (!Uri.TryCreate(uri, UriKind.Absolute, out var redirectUri))
                {
                    throw new RegistrationException("invalid_redirect_uri", $"RedirectUri '{uri}' is not valid.");
                }
                redirectUriList.Add(redirectUri);
            }

            ValidateUris(redirectUriList, registration.LogoUris, "invalid_logo_uri", "LogoUri");

            ValidateUris(redirectUriList, registration.PolicyUris, "invalid_policy_uri", "PolicyUri");
        }

        private void ValidateUris(IEnumerable<Uri> redirectUriList, IEnumerable<LocalizableProperty> localizableProperties, string errorCode, string uriName)
        {
            if (localizableProperties != null && localizableProperties.Any())
            {
                foreach (var uri in localizableProperties)
                {
                    if (!Uri.TryCreate(uri.Value, UriKind.Absolute, out var policyUri))
                    {
                        throw new RegistrationException(errorCode, $"{uriName} '{uri.Value}' is not valid.");
                    }
                    if (!redirectUriList.Any(u => u.Host == policyUri.Host))
                    {
                        throw new RegistrationException(errorCode, $"{uriName} '{uri.Value}' host doesn't match a redirect uri host.");
                    }
                }
            }
        }
    }
}
