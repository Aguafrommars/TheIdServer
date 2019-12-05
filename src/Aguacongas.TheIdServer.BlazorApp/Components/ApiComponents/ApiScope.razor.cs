using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiScope
    {
        [Parameter]
        public Entity.ProtectResource Model { get; set; }

        [Parameter]
        public Entity.ApiScope Scope { get; set; }

        [Parameter]
        public EventCallback<Entity.ApiScope> DeleteScopeClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.ApiScopeClaim> DeleteScopeClaimClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.ApiScopeClaim> ScopeClaimValueChanged { get; set; }

        private void OnDeleteScopeClicked()
        {
            DeleteScopeClicked.InvokeAsync(Scope);
        }

        private void OnScopeClaimDeleted(Entity.ApiScopeClaim claim)
        {
            DeleteScopeClaimClicked.InvokeAsync(claim);
        }

        private void OnScopeClaimValueChanged(Entity.ApiScopeClaim claim)
        {
            ScopeClaimValueChanged.InvokeAsync(claim);
        }
    }
}
