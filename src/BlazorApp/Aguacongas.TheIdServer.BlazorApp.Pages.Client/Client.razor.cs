// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Pages.Client.Extentions;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client
{
    public partial class Client
    {
        public static readonly string OIDC = "oidc";
        public static readonly string WSFED = "wsfed";
        public static readonly string SAML2P = "saml2p";

        private bool _filtered;
        private bool _isWebClient;

        protected override string Expand => $"{nameof(Entity.Client.IdentityProviderRestrictions)},{nameof(Entity.Client.ClientClaims)},{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.AllowedGrantTypes)},{nameof(Entity.Client.RedirectUris)},{nameof(Entity.Client.AllowedScopes)},{nameof(Entity.Client.Properties)},{nameof(Entity.Client.Resources)},{nameof(Entity.Client.AllowedIdentityTokenSigningAlgorithms)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "clients";

        protected override  async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            HandleModificationState.OnStateChange += OnStateChange;
        }

        protected void OnStateChange(ModificationKind kind, object entity)
        {
            var isWebClient = _isWebClient;
            _isWebClient = Model.IsWebClient();
            if (isWebClient != _isWebClient)
            {
                StateHasChanged();
            }
        }

        protected override Task<Entity.Client> Create()
        {
            return Task.FromResult(new Entity.Client
            {
                ProtocolType = "oidc",
                Enabled = true,
                EnableLocalLogin = true,
                AllowedGrantTypes = new List<Entity.ClientGrantType>(),
                AllowedIdentityTokenSigningAlgorithms = new List<Entity.ClientAllowedIdentityTokenSigningAlgorithm>(),
                AllowedScopes = new List<Entity.ClientScope>(),
                ClientClaims = new List<Entity.ClientClaim>(),
                ClientSecrets = new List<Entity.ClientSecret>(),
                IdentityProviderRestrictions = new List<Entity.ClientIdpRestriction>(),
                RedirectUris = new List<Entity.ClientUri>(),
                Properties = new List<Entity.ClientProperty>(),
                Resources = new List<Entity.ClientLocalizedResource>(),
                AbsoluteRefreshTokenLifetime = 86400,
                AccessTokenLifetime = 900,
                IdentityTokenLifetime = 300,
                AuthorizationCodeLifetime = 300,
                DeviceCodeLifetime = 300,
                SlidingRefreshTokenLifetime = 18000,
                CibaLifetime = 300,
                PollingInterval = 5,
                RefreshTokenUsage = (int)RefreshTokenUsage.OneTimeOnly
            });
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is Entity.Client client)
            {
                Model.Id = client.Id;
            }
            if (entity is Entity.IClientSubEntity subEntity)
            {
                subEntity.ClientId = Model.Id;
            }
            if (entity is Entity.ClientUri clientUri && clientUri.Uri != null)
            {
                if ((clientUri.Kind & Entity.UriKinds.Cors) == Entity.UriKinds.Cors)
                {
                    var corsUri = new Uri(clientUri.Uri);
                    clientUri.SanetizedCorsUri = $"{corsUri.Scheme.ToUpperInvariant()}://{corsUri.Host.ToUpperInvariant()}:{corsUri.Port}";
                    return;
                }
                clientUri.SanetizedCorsUri = null;
            }
            if (entity is Entity.ClientSecret secret && secret.Id == null && secret.Type == SecretTypes.SharedSecret)
            {
                secret.Value = secret.Value.Sha256();
            }
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is Entity.Client client)
            {
                client.IdentityProviderRestrictions = null;
                client.ClientClaims = null;
                client.ClientSecrets = null;
                client.AllowedGrantTypes = null;
                client.RedirectUris = null;
                client.AllowedScopes = null;
                client.Properties = null;
                client.Resources = null;
            }
        }

        protected override Task<object> UpdateAsync(Type entityType, object entity)
        {
            if (entity is Entity.ClientUri clientUri && string.IsNullOrWhiteSpace(clientUri.Uri))
            {
                return base.DeleteAsync(entityType, entity);
            }

            return base.UpdateAsync(entityType, entity);
        }

        protected override Task<object> CreateAsync(Type entityType, object entity)
        {
            if (entity is Entity.ClientUri clientUri && string.IsNullOrWhiteSpace(clientUri.Uri))
            {
                return Task.FromResult(entity);
            }
            return base.CreateAsync(entityType, entity);
        }

        protected override void OnCloning()
        {
            Model.ClientName = Localizer["Clone of {0}", Model.ClientName];
        }

        private void FilterFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                _isWebClient = Model.IsWebClient();
            }
            _filtered = hasFocus;
        }

        private bool IsWebClient()
        {
            if (_filtered)
            {
                return _isWebClient;
            }
            return Model.IsWebClient();
        }

        private static Entity.ClientSecret CreateSecret()
            => new()
            {
                Type = "SharedSecret"
            };

        private static Entity.ClientClaim CreateClaim()
            => new();

        private static Entity.ClientProperty CreateProperty()
            => new();

        private Task AddResource(Entity.EntityResourceKind kind)
        {
            var entity = new Entity.ClientLocalizedResource
            {
                ResourceKind = kind
            };
            Model.Resources.Add(entity);
            HandleModificationState.EntityCreated(entity);
            return Task.CompletedTask;
        }

        private void SetProtcolType(string protocolType)
        {
            Model.ProtocolType = protocolType;
            StateHasChanged();
        }
    }
}
