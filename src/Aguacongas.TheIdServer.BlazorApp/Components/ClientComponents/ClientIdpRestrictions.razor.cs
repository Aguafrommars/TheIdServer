using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientIdpRestrictions
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        [Parameter]
        public EventCallback<Entity.ClientIdpRestriction> ProviderDeletedClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.ClientIdpRestriction> ProviderValueChanged { get; set; }
    }
}
