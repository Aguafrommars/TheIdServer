using Aguacongas.TheIdServer.BlazorApp.Extensions;
using Aguacongas.TheIdServer.BlazorApp.Services;
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

        protected override string Expand => $"{nameof(Entity.Client.IdentityProviderRestrictions)},{nameof(Entity.Client.ClientClaims)},{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.AllowedGrantTypes)},{nameof(Entity.Client.RedirectUris)},{nameof(Entity.Client.AllowedScopes)},{nameof(Entity.Client.Properties)},{nameof(Entity.Client.Resources)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "clients";

        protected override void OnStateChange(ModificationKind kind, object entity)
        {
            _isWebClient = Model.IsWebClient();
            if (entity is Entity.ClientGrantType)
            {
                OnStateChange(kind, State.AllowedGrantTypes, entity);
            }
            if (entity is Entity.ClientScope)
            {
                OnStateChange(kind, State.AllowedScopes, entity);
            }
            if (entity is Entity.ClientClaim)
            {
                OnStateChange(kind, State.ClientClaims, entity);
            }
            if (entity is Entity.ClientSecret)
            {
                OnStateChange(kind, State.ClientSecrets, entity);
            }
            if (entity is Entity.ClientIdpRestriction)
            {
                OnStateChange(kind, State.IdentityProviderRestrictions, entity);
            }
            if (entity is Entity.ClientProperty)
            {
                OnStateChange(kind, State.Properties, entity);
            }
            if (entity is Entity.ClientLocalizedResource)
            {
                OnStateChange(kind, State.Resources, entity);
            }
            if (entity is Entity.ClientUri)
            {
                OnStateChange(kind, State.RedirectUris, entity);
            }
            base.OnStateChange(kind, entity);
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

        protected override Entity.Client CloneModel(Entity.Client entity)
        {
            var clone = base.CloneModel(entity);
            clone.IdentityProviderRestrictions = clone.IdentityProviderRestrictions.ToList();
            clone.ClientClaims = clone.ClientClaims.ToList();
            clone.ClientSecrets = clone.ClientSecrets.ToList();
            clone.AllowedGrantTypes = clone.AllowedGrantTypes.ToList();
            clone.RedirectUris = clone.RedirectUris.ToList();
            clone.AllowedScopes = clone.AllowedScopes.ToList();
            clone.Properties = clone.Properties.ToList();
            clone.Resources = clone.Resources.ToList();
            return clone;
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

        protected override Task OnFilterChanged(string term)
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

            return Task.CompletedTask;
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
