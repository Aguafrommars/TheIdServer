using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiClaimTypes
    {
        [Parameter]
        public ProtectResource Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        private void OnDeleteClaimClicked(ApiClaim claim)
        {
            Model.ApiClaims.Remove(claim);
            HandleModificationState.EntityDeleted(claim);
        }

        private void OnClaimValueChanged(ApiClaim claim)
        {
            claim.Api = Model;
            Model.ApiClaims.Add(new ApiClaim { Api = Model });
            HandleModificationState.EntityCreated(claim);
        }
    }
}
