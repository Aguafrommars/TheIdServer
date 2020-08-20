using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiClaimTypes
    {
        private IEnumerable<ApiClaim> Claims => Model.ApiClaims.Where(c => c.Type == null || c.Type.Contains(HandleModificationState.FilterTerm));
        private ApiClaim _claim = new ApiClaim();

        [Parameter]
        public ProtectResource Model { get; set; }

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
            if (entity is ApiClaim)
            {
                StateHasChanged();
            }
        }

        private void HandleModificationState_OnFilterChange(string term)
        {
            StateHasChanged();
        }

        private void OnDeleteClaimClicked(ApiClaim claim)
        {
            Model.ApiClaims.Remove(claim);
            HandleModificationState.EntityDeleted(claim);
        }

        private void OnClaimValueChanged(ApiClaim claim)
        {
            Model.ApiClaims.Add(claim);
            _claim = new ApiClaim();
            claim.Id = Guid.NewGuid().ToString();
            HandleModificationState.EntityCreated(claim);
        }
    }
}
