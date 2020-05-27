using Microsoft.AspNetCore.Components;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.LocalizedResourceComponents
{
    public partial class LocalizedResource
    {
        [Parameter]
        public Entity.LocalizedResource Model { get; set; }
    }
}
