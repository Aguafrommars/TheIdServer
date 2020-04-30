using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiScopeClaimTypes
    {
        [Parameter]
        public Entity.ApiScope Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        private void OnDeleteClaimClicked(Entity.ApiScopeClaim claim)
        {
            Model.ApiScopeClaims.Remove(claim);
            HandleModificationState.EntityDeleted(claim);
        }

        private void OnClaimValueChanged(Entity.ApiScopeClaim claim)
        {
            claim.ApiScpope = Model;
            Model.ApiScopeClaims.Add(new Entity.ApiScopeClaim { ApiScpope = Model });
            HandleModificationState.EntityCreated(claim);
        }
    }
}
