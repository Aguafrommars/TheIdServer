using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientUris
    {
        [Parameter]
        public Entity.Client Model { get; set; }

        private IEnumerable<ClientUri> GetClientUrls()
        {
            return Collection.Select(u => new ClientUri(u));
        }
    }
}
