// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientProperties
    {
        private IEnumerable<ClientProperty> Properties => Collection.Where(p => p.Id == null || (p.Key != null && p.Key.Contains(HandleModificationState.FilterTerm)) || (p.Value != null && p.Value.Contains(HandleModificationState.FilterTerm)));
    }
}
