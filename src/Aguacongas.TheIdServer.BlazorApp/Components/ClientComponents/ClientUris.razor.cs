using Aguacongas.TheIdServer.BlazorApp.Models;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientUris : ClientEntitiesGridModel<Entity.ClientUri>
    {
        private IEnumerable<ClientUri> GetClientUrls()
        {
            return Model.RedirectUris.Select(u => new ClientUri(u));
        }
    }
}
