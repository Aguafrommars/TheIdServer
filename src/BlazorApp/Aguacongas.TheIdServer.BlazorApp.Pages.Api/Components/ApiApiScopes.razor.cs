// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Api.Components
{
    public partial class ApiApiScopes
    {
        private IEnumerable<Entity.ApiApiScope> Scopes => Model.ApiScopes.Where(s => s.ApiScopeId == null || s.ApiScopeId.Contains(HandleModificationState.FilterTerm));
        private Entity.ApiApiScope _scope = new Entity.ApiApiScope();

        [Parameter]
        public Entity.ProtectResource Model { get; set; }

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
            if (entity is Entity.ApiApiScope)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string term)
        {
            StateHasChanged();
        }

        private void OnDeleteScopeClicked(Entity.ApiApiScope scope)
        {
            Model.ApiScopes.Remove(scope);
            HandleModificationState.EntityDeleted(scope);
        }

        private void OnScopeValueChanged(Entity.ApiApiScope scope)
        {
            Model.ApiScopes.Add(scope);
            _scope = new Entity.ApiApiScope { Api = Model };
            scope.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(scope);
        }
    }
}
