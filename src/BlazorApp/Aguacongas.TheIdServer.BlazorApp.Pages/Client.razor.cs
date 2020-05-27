using Aguacongas.TheIdServer.BlazorApp.Extensions;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Client
    {
        private bool _filtered;
        private bool _isWebClient;

        protected override string Expand => "IdentityProviderRestrictions,ClientClaims,ClientSecrets,AllowedGrantTypes,RedirectUris,AllowedScopes,Properties";

        protected override bool NonEditable => false;

        protected override string BackUrl => "clients";

        protected override void OnStateChange()
        {
            _isWebClient = Model.IsWebClient();
            base.OnStateChange();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            AddEmpyEntities();
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
                Properties = new List<Entity.ClientProperty>()
            });
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
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
                return;
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
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is Entity.IClientSubEntity subEntity)
            {
                subEntity.ClientId = Model.Id;
            }
        }

        protected override Type GetEntityType(FieldIdentifier identifier)
        {
            if (identifier.Model is ClientUri)
            {
                return typeof(Entity.ClientUri);
            }
            return base.GetEntityType(identifier);
        }

        protected override Entity.IEntityId GetEntityModel(FieldIdentifier identifier)
        {
            if (identifier.Model is ClientUri clientUri)
            {
                return clientUri.Parent;
            }
            return base.GetEntityModel(identifier);
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

        private void AddEmpyEntities()
        {
            Model.AllowedGrantTypes.Add(new Entity.ClientGrantType());
            Model.AllowedScopes.Add(new Entity.ClientScope());
            Model.IdentityProviderRestrictions.Add(new Entity.ClientIdpRestriction());
        }

        private void OnFilterChanged(string term)
        {
            Model.AllowedScopes = State.AllowedScopes
                .Where(s => s.Scope != null && s.Scope.Contains(term))
                .ToList();
            Model.AllowedGrantTypes = State.AllowedGrantTypes
                .Where(g => g.GrantType != null && g.GrantType.Contains(term))
                .ToList();
            Model.ClientClaims = State.ClientClaims
                .Where(c => (c.Type != null && c.Type.Contains(term)) || (c.Value != null && c.Value.Contains(term)))
                .ToList();
            Model.ClientSecrets = State.ClientSecrets
                .Where(s => (s.Description != null && s.Description.Contains(term)) || (s.Type != null && s.Type.Contains(term)))
                .ToList();
            Model.IdentityProviderRestrictions = State.IdentityProviderRestrictions
                .Where(i => i.Provider != null && i.Provider.Contains(term))
                .ToList();
            Model.RedirectUris = State.RedirectUris
                .Where(u => u.Uri != null && u.Uri.Contains(term))
                .ToList();
            Model.Properties = State.Properties
                .Where(p => p.Key.Contains(term) || p.Value.Contains(term))
                .ToList();

            AddEmpyEntities();
            StateHasChanged();
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

        private void OnScopeValueChanged(Entity.ClientScope scope)
        {
            EntityCreated(scope);
            Model.AllowedScopes.Add(new Entity.ClientScope());
            StateHasChanged();
        }

        private void OnScopeDeleted(Entity.ClientScope scope)
        {
            Model.AllowedScopes.Remove(scope);
            EntityDeleted(scope);
            StateHasChanged();
        }
    }
}
