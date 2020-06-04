using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiScope
    {
        [Parameter]
        public ICollection<Entity.ApiScope> Collection { get; set; }

        [Parameter]
        public Entity.ApiScope Scope { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            base.OnInitialized();
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            if (entity is Entity.ApiScope)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string term)
        {
            StateHasChanged();
        }

        private void OnDeleteScopeClicked()
        {
            Collection.Remove(Scope);
            HandleModificationState.EntityDeleted(Scope);
        }

        private Task AddResource(Entity.EntityResourceKind kind)
        {
            var entity = new Entity.ApiScopeLocalizedResource
            {
                ResourceKind = kind
            };
            Scope.Resources.Add(entity);
            HandleModificationState.EntityCreated(entity);
            return Task.CompletedTask;
        }
    }
}
