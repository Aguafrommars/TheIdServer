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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AddEmpyEntities();
        }

        protected override Entity.Client Create()
        {
            return new Entity.Client
            {
                ProtocolType = "oidc",
                AllowedGrantTypes = new List<Entity.ClientGrantType>(),
                AllowedScopes = new List<Entity.ClientScope>(),
                ClientClaims = new List<Entity.ClientClaim>(),
                ClientSecrets = new List<Entity.ClientSecret>(),
                IdentityProviderRestrictions = new List<Entity.ClientIdpRestriction>(),
                RedirectUris = new List<Entity.ClientUri>(),
                Properties = new List<Entity.ClientProperty>()
            };
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

        private void OnTokenTypeChanged(AccessTokenType accessTokenType)
        {
            Model.AccessTokenType = (int)accessTokenType;
            base.OnEntityUpdated(Model.GetType(), Model);
        }

        private void OnModelChanged()
        {
            base.OnEntityUpdated(Model.GetType(), Model);
        }

        private void OnAddUrlClicked()
        {
            var url = new Entity.ClientUri();
            Model.RedirectUris.Add(url);
            EntityCreated(url);
            StateHasChanged();
        }

        private void OnDeleteUrlClicked(Entity.ClientUri url)
        {
            Model.RedirectUris.Remove(url);
            EntityDeleted(url);
            StateHasChanged();
        }

        private void OnAddSecretClicked()
        {
            var secret = new Entity.ClientSecret
            {
                Type = "SharedSecret"
            };
            Model.ClientSecrets.Add(secret);
            EntityCreated(secret);
            StateHasChanged();
        }

        private void OnDeleteSecretClicked(Entity.ClientSecret secret)
        {
            Model.ClientSecrets.Remove(secret);
            EntityDeleted(secret);
            StateHasChanged();
        }

        private void OnAddClaimClicked()
        {
            var claim = new Entity.ClientClaim();
            Model.ClientClaims.Add(claim);
            EntityCreated(claim);
            StateHasChanged();
        }

        private void OnDeleteClaimClicked(Entity.ClientClaim claim)
        {
            Model.ClientClaims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }

        private void OnGrantTypeValueChanged(Entity.ClientGrantType grantType)
        {
            EntityCreated(grantType);
            Model.AllowedGrantTypes.Add(new Entity.ClientGrantType());
            _isWebClient = Model.IsWebClient();
            StateHasChanged();
        }

        private void OnGrantTypeDeleted(Entity.ClientGrantType grantType)
        {
            Model.AllowedGrantTypes.Remove(grantType);
            EntityDeleted(grantType);
            _isWebClient = Model.IsWebClient();
            StateHasChanged();
        }

        private void OnProviderValueChanged(Entity.ClientIdpRestriction provider)
        {
            EntityCreated(provider);
            Model.IdentityProviderRestrictions.Add(new Entity.ClientIdpRestriction());
            StateHasChanged();
        }

        private void OnProviderDeleted(Entity.ClientIdpRestriction provider)
        {
            Model.IdentityProviderRestrictions.Remove(provider);
            EntityDeleted(provider);
            StateHasChanged();
        }

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

        private void OnAddPropertyClicked()
        {
            var property = new Entity.ClientProperty();
            Model.Properties.Add(property);
            EntityCreated(property);
            StateHasChanged();
        }

        private void OnDeletePropertyClicked(Entity.ClientProperty property)
        {
            Model.Properties.Remove(property);
            EntityDeleted(property);
            StateHasChanged();
        }
    }
}
