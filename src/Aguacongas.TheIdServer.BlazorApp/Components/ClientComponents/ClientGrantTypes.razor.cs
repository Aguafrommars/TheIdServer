using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientGrantTypes
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        [Parameter]
        public EventCallback<Entity.ClientGrantType> GrantTypeDeletedClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.ClientGrantType> GrantTypeValueChanged { get; set; }
    }
}
