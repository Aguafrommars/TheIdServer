// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope
{
    public partial class ApiScope
    {
        protected override string Expand => $"{nameof(Entity.ApiScope.ApiScopeClaims)},{nameof(Entity.ApiScope.Properties)},{nameof(Entity.ApiScope.Resources)},{nameof(Entity.ApiScope.Apis)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "apiscopes";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override Task<Entity.ApiScope> Create() =>Task.FromResult(new Entity.ApiScope
            {
                Enabled = true,
                ApiScopeClaims = new List<Entity.ApiScopeClaim>(),
                Properties = new List<Entity.ApiScopeProperty>(),
                Resources = new List<Entity.ApiScopeLocalizedResource>()
            });        

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

        protected override void OnCloning()
        {
            Model.DisplayName = Localizer["Clone of {0}", Model.DisplayName];
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
