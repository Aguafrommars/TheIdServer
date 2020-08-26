// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.IdentityComponents
{
    public partial class IdentityClaimTypes
    {
        private IEnumerable<IdentityClaim> Claims => Model.IdentityClaims.Where(c => c.Type != null && c.Type.Contains(HandleModificationState.FilterTerm));
        private IdentityClaim _claim = new IdentityClaim();

        [Parameter]
        public IdentityResource Model { get; set; }

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
            if (entity is IdentityClaim)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }

        private void OnClaimDeletedClicked(IdentityClaim claim)
        {
            Model.IdentityClaims.Remove(claim);
            HandleModificationState.EntityDeleted(claim);
        }

        private void OnClaimValueChanged(IdentityClaim claim)
        {
            Model.IdentityClaims.Add(claim);
            _claim = new IdentityClaim();
            claim.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(claim);
        }
    }
}
