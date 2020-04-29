using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientGrantTypes
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        private void OnGrantTypeDeletedClicked(Entity.ClientGrantType grantType)
        {
            Model.AllowedGrantTypes.Remove(grantType);
            HandleModificationState.EntityDeleted(grantType);
        }

        private void OnGrantTypeValueChanged(Entity.ClientGrantType grantType)
        {
            Model.AllowedGrantTypes.Add(new Entity.ClientGrantType());
            if (grantType.GrantType == "implicit")
            {
                Model.AllowAccessTokensViaBrowser = true;
            }
            HandleModificationState.EntityCreated(grantType);
        }
    }
}
