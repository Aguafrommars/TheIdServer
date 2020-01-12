using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiClaimTypes
    {
        [Parameter]
        public ProtectResource Model { get; set; }

        [Parameter]
        public EventCallback<ApiClaim> DeleteClaimClicked { get; set; }

        [Parameter]
        public EventCallback<ApiClaim> ClaimValueChanged { get; set; }
    }
}
