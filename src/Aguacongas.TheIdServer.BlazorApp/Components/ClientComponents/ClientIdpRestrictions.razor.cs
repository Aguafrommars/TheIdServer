using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientIdpRestrictions
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }
        private void OnProviderDeletedClicked(Entity.ClientIdpRestriction restriction)
        {
            Model.IdentityProviderRestrictions.Remove(restriction);
            HandleModificationState.EntityDeleted(restriction);
        }

        private void OnProviderValueChanged(Entity.ClientIdpRestriction restriction)
        {
            Model.IdentityProviderRestrictions.Add(new Entity.ClientIdpRestriction());
            HandleModificationState.EntityCreated(restriction);
        }
    }
}
