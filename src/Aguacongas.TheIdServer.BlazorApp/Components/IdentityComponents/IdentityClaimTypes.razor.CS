using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.IdentityComponents
{
    public partial class IdentityClaimTypes
    {
        [Parameter]
        public IdentityResource Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        private void OnClaimDeletedClicked(IdentityClaim claim)
        {
            Model.IdentityClaims.Remove(claim);
            HandleModificationState.EntityDeleted(claim);
        }

        private void OnClaimValueChanged(IdentityClaim claim)
        {
            Model.IdentityClaims.Add(new IdentityClaim());
            HandleModificationState.EntityCreated(claim);
        }
    }
}
