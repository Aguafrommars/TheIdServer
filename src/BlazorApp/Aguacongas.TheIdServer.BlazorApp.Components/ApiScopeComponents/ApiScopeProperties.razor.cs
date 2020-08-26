// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiScopeComponents
{
    public partial class ApiScopeProperties
    {
        private IEnumerable<ApiScopeProperty> Properties => Collection.Where(p => p.Id == null || (p.Key != null && p.Key.Contains(HandleModificationState.FilterTerm)) || (p.Value != null && p.Value.Contains(HandleModificationState.FilterTerm)));
    }
}
