// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.User.Components
{
    public partial class UserRoles
    {
        private IEnumerable<Entity.Role> Collection => Model.Where(r => r.Name != null && r.Name.Contains(HandleModificationState.FilterTerm));
        private Entity.Role _role = new Entity.Role();

        [Parameter]
        public ICollection<Entity.Role> Model { get; set; }

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
            if (entity is Entity.Role)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }

        private void OnDeleteRoleClicked(Entity.Role role)
        {
            Model.Remove(role);
            HandleModificationState.EntityDeleted(role);
        }

        private void OnRoleValueChanged(Entity.Role role)
        {
            Model.Add(role);
            _role = new Entity.Role();
            role.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(role);
        }
    }
}
