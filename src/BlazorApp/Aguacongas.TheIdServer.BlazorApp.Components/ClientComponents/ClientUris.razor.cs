using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientUris
    {
        private IEnumerable<Entity.ClientUri> Uris => Collection.Where(u => u.Id == null || u.Uri != null && u.Uri.Contains(HandleModificationState.FilterTerm));

        [Parameter]
        public Entity.Client Model { get; set; }
    }
}
