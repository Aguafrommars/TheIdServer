// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientGrantTypes
    {
        private IEnumerable<Entity.ClientGrantType> GrantTypes => Model.AllowedGrantTypes.Where(g => g.GrantType != null && g.GrantType.Contains(HandleModificationState.FilterTerm));
        private Entity.ClientGrantType _grantType = new Entity.ClientGrantType();

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
            if (entity is Entity.ClientGrantType)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }

        private void OnGrantTypeDeletedClicked(Entity.ClientGrantType grantType)
        {
            Model.AllowedGrantTypes.Remove(grantType);
            HandleModificationState.EntityDeleted(grantType);
        }

        private void OnGrantTypeValueChanged(Entity.ClientGrantType grantType)
        {
            Model.AllowedGrantTypes.Add(grantType);
            if (grantType.GrantType == "implicit")
            {
                Model.AllowAccessTokensViaBrowser = true;
            }
            _grantType = new Entity.ClientGrantType();
            grantType.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(grantType);
        }
    }
}
