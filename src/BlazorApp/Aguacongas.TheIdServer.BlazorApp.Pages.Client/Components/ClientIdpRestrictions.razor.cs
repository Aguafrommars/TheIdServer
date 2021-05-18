// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientIdpRestrictions
    {
        private IEnumerable<Entity.ClientIdpRestriction> Restrictions => Model.IdentityProviderRestrictions.Where(i => i.Provider != null && i.Provider.Contains(HandleModificationState.FilterTerm));

        private Entity.ClientIdpRestriction _provider = new  Entity.ClientIdpRestriction();

        [Parameter]
        public Entity.Client Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
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

        private void OnProviderDeletedClicked(Entity.ClientIdpRestriction restriction)
        {
            Model.IdentityProviderRestrictions.Remove(restriction);
            HandleModificationState.EntityDeleted(restriction);
        }

        private void OnProviderValueChanged(Entity.ClientIdpRestriction restriction)
        {
            restriction.Id = Guid.NewGuid().ToString();
            Model.IdentityProviderRestrictions.Add(restriction);
            _provider = new Entity.ClientIdpRestriction();
            HandleModificationState.EntityCreated(restriction);
        }
    }
}
