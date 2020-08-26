// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.CultureComponents
{
    public partial class LocalizedResources
    {
        private IEnumerable<Entity.LocalizedResource> Resources => Collection.Where(p => p.Id == null || (p.Key != null && p.Key.Contains(HandleModificationState.FilterTerm)) || (p.Value != null && p.Value.Contains(HandleModificationState.FilterTerm)));

    }
}
