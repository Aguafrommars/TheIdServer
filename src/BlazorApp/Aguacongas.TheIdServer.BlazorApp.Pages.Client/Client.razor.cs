// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
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
        private bool _filtered;
        private bool _isWebClient;

        protected override string Expand => $"{nameof(Entity.Client.IdentityProviderRestrictions)},{nameof(Entity.Client.ClientClaims)},{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.AllowedGrantTypes)},{nameof(Entity.Client.RedirectUris)},{nameof(Entity.Client.AllowedScopes)},{nameof(Entity.Client.Properties)},{nameof(Entity.Client.Resources)}";

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
                AllowedGrantTypes = new List<Entity.ClientGrantType>(),
                AllowedScopes = new List<Entity.ClientScope>(),
                ClientClaims = new List<Entity.ClientClaim>(),
                ClientSecrets = new List<Entity.ClientSecret>(),
                IdentityProviderRestrictions = new List<Entity.ClientIdpRestriction>(),
                RedirectUris = new List<Entity.ClientUri>(),
                Properties = new List<Entity.ClientProperty>(),
                Resources = new List<Entity.ClientLocalizedResource>()
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

        protected override void OnEntityUpdated(Type entityType, Entity.IEntityId entityModel)
        {
            if (entityType == typeof(Entity.ClientGrantType))
            {
                return;
            }
            base.OnEntityUpdated(entityType, entityModel);
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

        private Entity.ClientSecret CreateSecret()
            => new Entity.ClientSecret
            {
                Type = "SharedSecret"
            };

        private Entity.ClientUri CreateRedirectUri()
            => new Entity.ClientUri();

        private Entity.ClientClaim CreateClaim()
            => new Entity.ClientClaim();

        private Entity.ClientProperty CreateProperty()
            => new Entity.ClientProperty();

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
    }
}
