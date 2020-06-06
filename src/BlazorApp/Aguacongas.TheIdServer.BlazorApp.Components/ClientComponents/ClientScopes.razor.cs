using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientScopes
    {
        private IEnumerable<Entity.ClientScope> Scopes => Model.AllowedScopes.Where(s => s.Scope != null && s.Scope.Contains(HandleModificationState.FilterTerm));
        private Entity.ClientScope _scope = new Entity.ClientScope();

        [Parameter]
        public Entity.Client Model { get; set; }

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
            if (entity is Entity.ClientScope)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }

        private void OnScopeValueChanged(Entity.ClientScope scope)
        {
            Model.AllowedScopes.Add(scope);
            _scope = new Entity.ClientScope();
            scope.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(scope);
        }

        private void OnScopeDeleted(Entity.ClientScope scope)
        {
            Model.AllowedScopes.Remove(scope);
            HandleModificationState.EntityDeleted(scope);
        }
    }
}
