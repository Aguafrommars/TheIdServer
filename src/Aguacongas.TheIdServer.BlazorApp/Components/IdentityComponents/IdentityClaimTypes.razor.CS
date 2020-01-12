using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.IdentityComponents
{
    public partial class IdentityClaimTypes
    {
        [Parameter]
        public IdentityResource Model { get; set; }

        [Parameter]
        public EventCallback<IdentityClaim> DeleteClaimClicked { get; set; }

        [Parameter]
        public EventCallback<IdentityClaim> ClaimValueChanged { get; set; }
    }
}
