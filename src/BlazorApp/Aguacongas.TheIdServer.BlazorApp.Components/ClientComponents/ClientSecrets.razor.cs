using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientSecrets
    {
        private IEnumerable<Entity.ClientSecret> Secrets => Collection.Where(s => s.Id == null || (s.Description != null && s.Description.Contains(HandleModificationState.FilterTerm)) || (s.Type != null && s.Type.Contains(HandleModificationState.FilterTerm)));
    }
}
