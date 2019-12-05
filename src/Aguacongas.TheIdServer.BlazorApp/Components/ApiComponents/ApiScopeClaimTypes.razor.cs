using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiScopeClaimTypes
    {
        [Parameter]
        public Entity.ApiScope Scope { get; set; }

        [Parameter]
        public EventCallback<Entity.ApiScopeClaim> DeleteClaimClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.ApiScopeClaim> ClaimValueChanged { get; set; }
    }
}
