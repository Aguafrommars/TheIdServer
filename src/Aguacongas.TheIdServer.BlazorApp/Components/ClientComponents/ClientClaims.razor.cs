using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientClaims
    {
        [Parameter]
        public Entity.Client Model { get; set; }
    }
}
