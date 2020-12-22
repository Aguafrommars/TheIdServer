// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope
{
    public partial class ApiScope
    {
        protected override string Expand => $"{nameof(Entity.ApiScope.ApiScopeClaims)},{nameof(Entity.ApiScope.Properties)},{nameof(Entity.ApiScope.Resources)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "scopes";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override Task<Entity.ApiScope> Create()
        {
            return Task.FromResult(new Entity.ApiScope
            {
                ApiScopeClaims = new List<Entity.ApiScopeClaim>(),
                Properties = new List<Entity.ApiScopeProperty>(),
                Resources = new List<Entity.ApiScopeLocalizedResource>()
            });
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is Entity.ApiScope identity)
            {
                identity.ApiScopeClaims = null;
                identity.Properties = null;
                identity.Resources = null;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is Entity.ApiScope identity)
            {
                Model.Id = identity.Id;
            }
            if (entity is Entity.IApiScopeSubEntity subEntity)
            {
                subEntity.ApiScopeId = Model.Id;
            }
        }

        private static Entity.ApiScopeProperty CreateProperty()
            => new();

        private Task AddResource(Entity.EntityResourceKind kind)
        {
            var entity = new Entity.ApiScopeLocalizedResource
            {
                ResourceKind = kind
            };
            Model.Resources.Add(entity);
            HandleModificationState.EntityCreated(entity);
            return Task.CompletedTask;
        }
    }
}
