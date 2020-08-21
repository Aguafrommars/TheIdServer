using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Options;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Errors.Model;
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
        private readonly IdentityServer4.Configuration.IdentityServerOptions _identityServerOptions1;
        private readonly DynamicClientRegistrationOptions _dymamicClientRegistrationOptions;
        private readonly IAdminStore<Client> _clientStore;
        private readonly IAdminStore<ClientUri> _clientUriStore;
        private readonly IAdminStore<ClientLocalizedResource> _clientResourceStore;
        private readonly IAdminStore<ClientProperty> _clientPropertyStore;
        private readonly IAdminStore<ClientGrantType> _clientGrantTypeStore;
        private readonly IDiscoveryResponseGenerator _discoveryResponseGenerator;
        private readonly IdentityServer4.Models.Client _defaultValues = new IdentityServer4.Models.Client();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterClientService" /> class.
        /// </summary>
        /// <param name="clientStore">The client store.</param>
        /// <param name="clientUriStore">The client URI store.</param>
        /// <param name="clientResourceStore">The client resource store.</param>
        /// <param name="clientGrantTypeStore">The client grant type store.</param>
        /// <param name="clientPropertyStore">The client property store.</param>
        /// <param name="discoveryResponseGenerator">The discovery response generator.</param>
        /// <param name="identityServerOptions">The options.</param>
        /// <param name="dymamicClientRegistrationOptions">The dymamic client registration options.</param>
        /// <exception cref="ArgumentNullException">options
        /// or
        /// clientStore
        /// or
        /// clientUriStore
        /// or
        /// clientResourceStore
        /// or
        /// clientPropertyStore
        /// or
        /// clientGrantTypeStore
        /// or
        /// discoveryResponseGenerator</exception>
        public RegisterClientService(IAdminStore<Client> clientStore,
            IAdminStore<ClientUri> clientUriStore,
            IAdminStore<ClientLocalizedResource> clientResourceStore,
            IAdminStore<ClientGrantType> clientGrantTypeStore,
            IAdminStore<ClientProperty> clientPropertyStore,
            IDiscoveryResponseGenerator discoveryResponseGenerator, 
            IdentityServer4.Configuration.IdentityServerOptions identityServerOptions,
            IOptions<DynamicClientRegistrationOptions> dymamicClientRegistrationOptions)

        {
            _identityServerOptions1 = identityServerOptions ?? throw new ArgumentNullException(nameof(identityServerOptions));
            _dymamicClientRegistrationOptions = dymamicClientRegistrationOptions?.Value ?? throw new ArgumentNullException(nameof(dymamicClientRegistrationOptions));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            _clientUriStore = clientUriStore ?? throw new ArgumentNullException(nameof(clientUriStore));
            _clientResourceStore = clientResourceStore ?? throw new ArgumentNullException(nameof(clientResourceStore));
            _clientPropertyStore = clientPropertyStore ?? throw new ArgumentNullException(nameof(clientPropertyStore));
            _clientGrantTypeStore = clientGrantTypeStore ?? throw new ArgumentNullException(nameof(clientGrantTypeStore));
            _discoveryResponseGenerator = discoveryResponseGenerator ?? throw new ArgumentNullException(nameof(discoveryResponseGenerator));
        }

        /// <summary>
        /// Registers the asynchronous.
        /// </summary>
        /// <param name="registration">The client registration.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns></returns>
        public async Task<ClientRegisteration> RegisterAsync(ClientRegisteration registration, HttpContext httpContext)
        {
            var uri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/";
            var discovery = await _discoveryResponseGenerator.CreateDiscoveryDocumentAsync(uri, uri).ConfigureAwait(false);
            ValidateCaller(registration, httpContext);
            Validate(registration, discovery);

            var secret = Guid.NewGuid().ToString();
            var clientName = registration.ClientNames?.FirstOrDefault(n => n.Culture == null)?.Value ??
                    registration.ClientNames?.FirstOrDefault()?.Value ?? Guid.NewGuid().ToString();
            var existing = await _clientStore.GetAsync(clientName, null).ConfigureAwait(false);
            registration.Id = existing != null ? Guid.NewGuid().ToString() : clientName;
            registration.Id = registration.Id.Contains(' ') ? Guid.NewGuid().ToString() : registration.Id;

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
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        Scope = "address"
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        Scope = "email"
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        Scope = "phone"
                    }
                },
                AllowRememberConsent = _defaultValues.AllowRememberConsent,
                AllowPlainTextPkce = _defaultValues.AllowPlainTextPkce,
                AlwaysIncludeUserClaimsInIdToken = _defaultValues.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = _defaultValues.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = _defaultValues.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = !string.IsNullOrEmpty(_defaultValues.BackChannelLogoutUri),
                BackChannelLogoutUri = _defaultValues.BackChannelLogoutUri,
                ClientClaimsPrefix = _defaultValues.ClientClaimsPrefix,
                ClientName = clientName,
                ClientSecrets = new List<ClientSecret>
                {
                    new ClientSecret
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "SharedSecret",
                        Value = IdentityServer4.Models.HashExtensions.Sha256(secret)
                    }
                },
                ClientUri = registration.ClientUris?.FirstOrDefault(u => u.Culture == null)?.Value ?? registration.ClientUris?.FirstOrDefault()?.Value,
                ConsentLifetime = _defaultValues.ConsentLifetime,
                Description = _defaultValues.Description,
                DeviceCodeLifetime = _defaultValues.DeviceCodeLifetime,
                Enabled = true,
                EnableLocalLogin = _defaultValues.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = !string.IsNullOrEmpty(_defaultValues.FrontChannelLogoutUri),
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
                RequirePkce = false,
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
                    .Union(registration.LogoUris != null ? registration.LogoUris.Where(u => u.Culture != null).Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.LogoUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .Union(registration.PolicyUris != null ? registration.PolicyUris.Where(u => u.Culture != null).Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.PolicyUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .Union(registration.TosUris != null ? registration.TosUris.Where(u => u.Culture != null).Select(u => new ClientLocalizedResource
                    {
                        Id = Guid.NewGuid().ToString(),
                        CultureId = u.Culture,
                        ResourceKind = EntityResourceKind.TosUri,
                        Value = u.Value
                    }) : new List<ClientLocalizedResource>(0))
                    .ToList(),
                Properties = new List<ClientProperty>
                {
                    new ClientProperty
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = "applicationType",
                        Value = registration.ApplicationType
                    }
                }.Union(registration.Contacts != null ? new List<ClientProperty>
                {
                    new ClientProperty
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = "contacts",
                        Value = string.Join("; ", registration.Contacts)
                    }
                } : new List<ClientProperty>(0))
                .ToList(),
                SlidingRefreshTokenLifetime = _defaultValues.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = _defaultValues.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = _defaultValues.UserCodeType,
                UserSsoLifetime = _defaultValues.UserSsoLifetime,
                PolicyUri = registration.TosUris?.FirstOrDefault(u => u.Culture == null)?.Value ?? registration.TosUris?.FirstOrDefault()?.Value,
                TosUri = registration.TosUris?.FirstOrDefault(u => u.Culture == null)?.Value ?? registration.TosUris?.FirstOrDefault()?.Value,
            };

            client.RegistrationToken = Guid.NewGuid();

            await _clientStore.CreateAsync(client).ConfigureAwait(false);

            registration.RegistrationToken = client.RegistrationToken.ToString();
            registration.RegistrationUri = $"{discovery["registration_endpoint"]}/{client.Id}";
            registration.JwksUri = discovery["jwks_uri"].ToString();
            registration.ClientSecret = secret;
            registration.ClientSecretExpireAt = secret != null ? (int?)0 : null;

            return registration;
        }

        /// <summary>
        /// Updates the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="registration">The client.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ClientRegisteration> UpdateRegistrationAsync(string clientId, ClientRegisteration registration, string uri)
        {
            var client = await GetClientAsync(clientId).ConfigureAwait(false);

            var discovery = await _discoveryResponseGenerator.CreateDiscoveryDocumentAsync(uri, uri).ConfigureAwait(false);
            Validate(registration, discovery);
            await UpdateClient(registration, client).ConfigureAwait(false);
            await UpdateRedirectUris(clientId, registration).ConfigureAwait(false);
            await UpdateGrantTypes(clientId, registration).ConfigureAwait(false);

            var resourceResponse = await _clientResourceStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ClientGrantType.ClientId)} eq '{clientId}'"
            }).ConfigureAwait(false);

            var items = resourceResponse.Items;
            var clientNameList = registration.ClientNames?.Where(n => n.Culture != null) ?? Array.Empty<LocalizableProperty>();
            var clientUriList = registration.ClientUris?.Where(u => u.Culture != null) ?? Array.Empty<LocalizableProperty>();
            var logoUriList = registration.LogoUris?.Where(u => u.Culture != null) ?? Array.Empty<LocalizableProperty>();
            var policyUriList = registration.PolicyUris?.Where(u => u.Culture != null) ?? Array.Empty<LocalizableProperty>();
            var tosUriList = registration.TosUris?.Where(u => u.Culture != null) ?? Array.Empty<LocalizableProperty>();

            foreach (var item in items)
            {
                await DeleteItemAsync(clientNameList, clientUriList, logoUriList, policyUriList, tosUriList, item).ConfigureAwait(false);
            }

            await AddResourceAsync(clientId, items, clientNameList).ConfigureAwait(false);
            await AddResourceAsync(clientId, items, clientUriList).ConfigureAwait(false);
            await AddResourceAsync(clientId, items, logoUriList).ConfigureAwait(false);
            await AddResourceAsync(clientId, items, policyUriList).ConfigureAwait(false);
            await AddResourceAsync(clientId, items, tosUriList).ConfigureAwait(false);

            await UpdatePropertiesAsync(clientId, registration).ConfigureAwait(false);

            registration.RegistrationToken = null;
            registration.RegistrationUri = null;
            registration.JwksUri = discovery["jwks_uri"].ToString();
            registration.ClientSecret = client.ClientSecrets.FirstOrDefault()?.Value;
            registration.ClientSecretExpireAt = client.ClientSecrets.Any() ? (int?)0 : null;

            return registration;
        }

        /// <summary>
        /// Gets the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ClientRegisteration> GetRegistrationAsync(string clientId, string uri)
        {
            var client = await GetClientAsync(clientId).ConfigureAwait(false);

            var discovery = await _discoveryResponseGenerator.CreateDiscoveryDocumentAsync(uri, uri).ConfigureAwait(false);

            return new ClientRegisteration
            {
                Id = clientId,
                ApplicationType = client.Properties.FirstOrDefault(p => p.Key == "applicationType")?.Value ?? "web",
                ClientNames = client.Resources.Where(r => r.ResourceKind == EntityResourceKind.DisplayName).Select(r => new LocalizableProperty
                {
                    Culture = r.CultureId,
                    Value = r.Value
                }).Union(new[] { new LocalizableProperty { Value = client.ClientName } }),
                ClientSecret = client.ClientSecrets.FirstOrDefault()?.Value,
                ClientSecretExpireAt = client.ClientSecrets.Any() ? (int?)0 : null,
                ClientUris = client.Resources.Where(r => r.ResourceKind == EntityResourceKind.ClientUri).Select(r => new LocalizableProperty
                {
                    Culture = r.CultureId,
                    Value = r.Value
                }).Union(new[] { new LocalizableProperty { Value = client.ClientUri } }),
                Contacts = client.Properties.FirstOrDefault(p => p.Key == "contacts")?.Value.Split("; "),
                GrantTypes = client.AllowedGrantTypes.Select(g => g.GrantType),
                JwksUri = discovery["jwks_uri"].ToString(),
                LogoUris = client.Resources.Where(r => r.ResourceKind == EntityResourceKind.LogoUri).Select(r => new LocalizableProperty
                {
                    Culture = r.CultureId,
                    Value = r.Value
                }).Union(new[] { new LocalizableProperty { Value = client.LogoUri } }),
                PolicyUris = client.Resources.Where(r => r.ResourceKind == EntityResourceKind.PolicyUri).Select(r => new LocalizableProperty
                {
                    Culture = r.CultureId,
                    Value = r.Value
                }).Union(new[] { new LocalizableProperty { Value = client.PolicyUri } }),
                RedirectUris = client.RedirectUris.Select(u => u.Uri),
                ResponseTypes = client.Properties.FirstOrDefault(p => p.Key == "responseType")?.Value.Split("; "),
                TosUris = client.Resources.Where(r => r.ResourceKind == EntityResourceKind.TosUri).Select(r => new LocalizableProperty
                {
                    Culture = r.CultureId,
                    Value = r.Value
                }).Union(new[] { new LocalizableProperty { Value = client.TosUri } })
            };
        }

        /// <summary>
        /// Deletes the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task DeleteRegistrationAsync(string clientId)
            => _clientStore.DeleteAsync(clientId);

        private void ValidateCaller(ClientRegisteration registration, HttpContext httpContext)
        {
            if (!(httpContext.User?.IsInRole(SharedConstants.WRITER) ?? false))
            {
                var allowedContact = _dymamicClientRegistrationOptions.AllowedContacts?.FirstOrDefault(c => registration.Contacts?.Contains(c.Contact) ?? false);
                if (allowedContact == null)
                {
                    throw new ForbiddenException();
                }
                if (!allowedContact.AllowedHosts.Any(h => registration.RedirectUris.Select(u => new Uri(u)).Any(u => u.Host == h)))
                {
                    throw new ForbiddenException();
                }
            }
        }

        private async Task DeleteItemAsync(IEnumerable<LocalizableProperty> clientNameList, IEnumerable<LocalizableProperty> clientUriList, IEnumerable<LocalizableProperty> logoUriList, IEnumerable<LocalizableProperty> policyUriList, IEnumerable<LocalizableProperty> tosUriList, ClientLocalizedResource item)
        {
            if (item.ResourceKind == EntityResourceKind.DisplayName && !clientNameList.Any(IsDeleted(item)))
            {
                await _clientResourceStore.DeleteAsync(item.Id).ConfigureAwait(false);
            }
            if (item.ResourceKind == EntityResourceKind.ClientUri && !clientUriList.Any(IsDeleted(item)))
            {
                await _clientResourceStore.DeleteAsync(item.Id).ConfigureAwait(false);
            }
            if (item.ResourceKind == EntityResourceKind.LogoUri && !logoUriList.Any(IsDeleted(item)))
            {
                await _clientResourceStore.DeleteAsync(item.Id).ConfigureAwait(false);
            }
            if (item.ResourceKind == EntityResourceKind.PolicyUri && !policyUriList.Any(IsDeleted(item)))
            {
                await _clientResourceStore.DeleteAsync(item.Id).ConfigureAwait(false);
            }
            if (item.ResourceKind == EntityResourceKind.TosUri && !tosUriList.Any(n => n.Culture == item.CultureId && n.Value == item.Value))
            {
                await _clientResourceStore.DeleteAsync(item.Id).ConfigureAwait(false);
            }
        }

        private static Func<LocalizableProperty, bool> IsDeleted(ClientLocalizedResource item)
        {
            return n => n.Culture == item.CultureId && n.Value == item.Value;
        }

        private async Task<Client> GetClientAsync(string clientId)
        {
            var client = await _clientStore.GetAsync(clientId, new GetRequest
            {
                Expand = $"{nameof(Client.AllowedGrantTypes)},{nameof(Client.RedirectUris)},{nameof(Client.Resources)},{nameof(Client.Properties)},{nameof(Client.ClientSecrets)}"
            }).ConfigureAwait(false);
            if (client == null)
            {
                throw new NotFoundException($"{clientId} not found.");
            }

            return client;
        }

        private async Task UpdatePropertiesAsync(string clientId, ClientRegisteration registration)
        {
            var propertiesResponse = await _clientPropertyStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ClientProperty.ClientId)} eq '{clientId}' and ({nameof(ClientProperty.Key)} eq 'contacts' or {nameof(ClientProperty.Key)} eq 'responseTypes')"
            }).ConfigureAwait(false);
            await UpdatePropertyAsync(clientId, registration, propertiesResponse, registration.Contacts != null ? string.Join("; ", registration.Contacts) : null, "contacts").ConfigureAwait(false);
            await UpdatePropertyAsync(clientId, registration, propertiesResponse, registration.ResponseTypes != null ? string.Join("; ", registration.ResponseTypes) : null, "responseType").ConfigureAwait(false);
        }

        private async Task UpdatePropertyAsync(string clientId, ClientRegisteration registration, PageResponse<ClientProperty> propertiesResponse, string values, string key)
        {
            var property = propertiesResponse.Items.FirstOrDefault(p => p.Key == key);

            if ((registration.Contacts == null || !registration.Contacts.Any()) && property != null)
            {
                await _clientPropertyStore.DeleteAsync(property.Id).ConfigureAwait(false);
            }
            if (registration.Contacts == null || !registration.Contacts.Any())
            {
                if (property == null)
                {
                    await _clientPropertyStore.CreateAsync(new ClientProperty
                    {
                        ClientId = clientId,
                        Id = Guid.NewGuid().ToString(),
                        Key = key,
                        Value = values
                    }).ConfigureAwait(false);
                }
                else if (property.Value != values)
                {
                    property.Value = values;
                    await _clientPropertyStore.UpdateAsync(property).ConfigureAwait(false);
                }
            }
        }

        private async Task AddResourceAsync(string clientId, IEnumerable<ClientLocalizedResource> items, IEnumerable<LocalizableProperty> propertyList)
        {
            foreach (var property in propertyList)
            {
                if (!items.Any(i => i.ResourceKind == EntityResourceKind.DisplayName && i.Value != property.Value))
                {
                    await _clientResourceStore.CreateAsync(new ClientLocalizedResource
                    {
                        ClientId = clientId,
                        Id = Guid.NewGuid().ToString(),
                        ResourceKind = EntityResourceKind.DisplayName,
                        CultureId = property.Culture,
                        Value = property.Value
                    }).ConfigureAwait(false);
                }
            }
        }

        private async Task UpdateGrantTypes(string clientId, ClientRegisteration registration)
        {
            var grantTypeResponse = await _clientGrantTypeStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ClientGrantType.ClientId)} eq '{clientId}'"
            }).ConfigureAwait(false);

            foreach (var item in grantTypeResponse.Items)
            {
                if (!registration.GrantTypes.Any(g => g == item.GrantType))
                {
                    await _clientGrantTypeStore.DeleteAsync(item.Id).ConfigureAwait(false);
                }
            }

            foreach (var grantType in registration.GrantTypes)
            {
                if (!grantTypeResponse.Items.Any(u => u.GrantType == grantType))
                {
                    await _clientGrantTypeStore.CreateAsync(new ClientGrantType
                    {
                        ClientId = clientId,
                        Id = Guid.NewGuid().ToString(),
                        GrantType = grantType
                    }).ConfigureAwait(false);
                }
            }
        }

        private async Task UpdateRedirectUris(string clientId, ClientRegisteration registration)
        {
            var redirectUriResponse = await _clientUriStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ClientUri.ClientId)} eq '{clientId}'"
            }).ConfigureAwait(false);

            foreach (var item in redirectUriResponse.Items)
            {
                if (!registration.RedirectUris.Any(u => u == item.Uri))
                {
                    await _clientUriStore.DeleteAsync(item.Id).ConfigureAwait(false);
                }
            }

            foreach (var redirectUri in registration.RedirectUris)
            {
                if (!redirectUriResponse.Items.Any(u => u.Uri == redirectUri))
                {
                    await _clientUriStore.CreateAsync(new ClientUri
                    {
                        ClientId = clientId,
                        Id = Guid.NewGuid().ToString(),
                        Kind = UriKinds.Cors | UriKinds.Redirect,
                        Uri = redirectUri
                    }).ConfigureAwait(false);
                }
            }
        }

        private async Task UpdateClient(ClientRegisteration registration, Client client)
        {
            client.ClientUri = registration.ClientUris?.FirstOrDefault(u => u.Culture == null)?.Value ??
                            registration.ClientUris?.FirstOrDefault()?.Value;
            client.LogoUri = registration.LogoUris?.FirstOrDefault(u => u.Culture == null)?.Value ??
                registration.LogoUris?.FirstOrDefault()?.Value;
            client.PolicyUri = registration.TosUris?.FirstOrDefault(u => u.Culture == null)?.Value ??
                registration.TosUris?.FirstOrDefault()?.Value;
            client.TosUri = registration.TosUris?.FirstOrDefault(u => u.Culture == null)?.Value ??
                registration.TosUris?.FirstOrDefault()?.Value;

            await _clientStore.UpdateAsync(client).ConfigureAwait(false);
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

            var validationOptions = _identityServerOptions1.Validation;
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

            foreach (var localizable in localizableProperties)
            {
                if (!Uri.TryCreate(localizable.Value, UriKind.Absolute, out var uri))
                {
                    throw new RegistrationException(errorCode, $"{uriName} '{localizable.Value}' is not valid.");
                }
                if (!redirectUriList.Any(u => u.Host == uri.Host))
                {
                    throw new RegistrationException(errorCode, $"{uriName} '{localizable.Value}' host doesn't match a redirect uri host.");
                }
            }
        }
    }
}
