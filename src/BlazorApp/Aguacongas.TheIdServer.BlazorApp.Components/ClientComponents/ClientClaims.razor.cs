// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientClaims
    {
        private IEnumerable<ClientClaim> Claims => Collection.Where(c => c.Id == null || (c.Type != null && c.Type.Contains(HandleModificationState.FilterTerm)) || (c.Value != null && c.Value.Contains(HandleModificationState.FilterTerm)));

        [Parameter]
        public Client Model { get; set; }
    }
}
