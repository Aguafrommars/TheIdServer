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
            var discovery = await _discoveryResponseGenerator.CreateDiscoveryDocumentAsync(uri, uri).ConfigureAwait(false);
            Validate(registration, discovery);

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

            registration.JwksUri = discovery["jwks_uri"].ToString();
            return registration;
        }

        private void Validate(ClientRegisteration registration, Dictionary<string, object> discovery)
        {
            registration.GrantTypes ??= new List<string>
            {
                "authorization_code"
            };

            if (registration.RedirectUris == null || !registration.RedirectUris.Any())
            {
                throw new RegistrationException("invalid_redirect_uri", "RedirectUri is required.");
            }

            if (registration.ApplicationType != "web" && registration.ApplicationType != "native")
            {
                throw new RegistrationException("invalid_application_type", $"ApplicationType '{registration.ApplicationType}' is invalid. It must be 'web' or 'native'.");
            }

            var validationOptions = _options.Validation;
            var redirectUriList = new List<Uri>(registration.RedirectUris.Count());
            foreach (var uri in registration.RedirectUris)
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

                ValidateRedirectUri(registration, uri, redirectUri);

                redirectUriList.Add(redirectUri);
            }

            ValidateUris(redirectUriList, registration.LogoUris, "invalid_logo_uri", "LogoUri");

            ValidateUris(redirectUriList, registration.PolicyUris, "invalid_policy_uri", "PolicyUri");

            if (registration.GrantTypes == null || !registration.GrantTypes.Any())
            {
                registration.GrantTypes = new[] { "authorization_code" };
            }
            ValidateGrantType(registration.GrantTypes, discovery["grant_types_supported"] as IEnumerable<string>);

            if (registration.ResponseTypes == null || !registration.ResponseTypes.Any())
            {
                registration.ResponseTypes = new[] { "code" };
            }

            ValidateResponseType(registration.GrantTypes, registration.ResponseTypes, discovery["response_types_supported"] as IEnumerable<string>);
        }

        private static void ValidateRedirectUri(ClientRegisteration registration, string uri, Uri redirectUri)
        {
            if (registration.ApplicationType == "web" &&
                                registration.GrantTypes.Contains("implicit"))
            {
                if (redirectUri.Scheme != "https")
                {
                    throw new RegistrationException("invalid_redirect_uri", $"Invalid RedirectUri '{uri}'. Implicit client must use 'https' scheme only.");
                }
                if (redirectUri.Host.ToUpperInvariant() == "LOCALHOST")
                {
                    throw new RegistrationException("invalid_redirect_uri", $"Invalid RedirectUri '{uri}'. Implicit client cannot use 'localhost' host.");
                }
            }
            if (registration.ApplicationType == "native")
            {
                if (redirectUri.Scheme == Uri.UriSchemeGopher ||
                    redirectUri.Scheme == Uri.UriSchemeHttps ||
                    redirectUri.Scheme == Uri.UriSchemeNews ||
                    redirectUri.Scheme == Uri.UriSchemeNntp)
                {
                    throw new RegistrationException("invalid_redirect_uri", $"Invalid RedirectUri '{uri}'.Native client cannot use standard '{redirectUri.Scheme}' scheme, you must use a custom scheme such as 'net.pipe' or 'net.tcp', or 'http' scheme with 'localhost' host.");
                }
                if (redirectUri.Scheme == Uri.UriSchemeHttp && redirectUri.Host.ToUpperInvariant() != "LOCALHOST")
                {
                    throw new RegistrationException("invalid_redirect_uri", $"Invalid RedirectUri '{uri}'.Only 'localhost' host is allowed for 'http' scheme and 'native' client.");
                }
            }
        }

        private void ValidateResponseType(IEnumerable<string> grantTypes, IEnumerable<string> responseTypes, IEnumerable<string> responseTypesSupported)
        {
            foreach (var responseType in responseTypes)
            {
                var responseTypeSegmentList = responseType.Split(' ');
                foreach (var type in responseTypeSegmentList)
                {
                    if (!responseTypesSupported.Any(t => t.Split(' ').Length == responseTypeSegmentList.Length && t.Contains(type)))
                    {
                        throw new RegistrationException("invalid_response_type", $"ResponseType '{responseType}' is not supported.");
                    }
                    switch (type)
                    {
                        case "code":
                            if (!grantTypes.Any(g => g == "authorization_code"))
                            {
                                throw new RegistrationException("invalid_response_type", $"No GrantType 'authorization_code' for ResponseType '{responseType}' found in grant_types.");
                            }
                            break;
                        case "token":
                        case "id_token":
                            if (!grantTypes.Any(g => g == "implicit"))
                            {
                                throw new RegistrationException("invalid_response_type", $"No GrantType 'implicit' for ResponseType '{responseType}' found in grant_types.");
                            }
                            break;
                    }
                }
            }
        }

        private void ValidateGrantType(IEnumerable<string> grantTypes, IEnumerable<string> grantTypesSupported)
        {
            foreach (var grantType in grantTypes)
            {
                if (!grantTypesSupported.Contains(grantType))
                {
                    throw new RegistrationException("invalid_grant_type", $"GrantType '{grantType}' is not supported.");
                }
            }
        }

        private void ValidateUris(IEnumerable<Uri> redirectUriList, IEnumerable<LocalizableProperty> localizableProperties, string errorCode, string uriName)
        {
            if (localizableProperties == null)
            {
                return;
            }

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
