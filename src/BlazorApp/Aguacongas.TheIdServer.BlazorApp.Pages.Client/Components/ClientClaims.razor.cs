// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientClaims
    {
        private IEnumerable<Entity.ClientClaim> Claims => Collection.Where(c => c.Id == null || (c.Type != null && c.Type.Contains(HandleModificationState.FilterTerm)) || (c.Value != null && c.Value.Contains(HandleModificationState.FilterTerm)));

        [Parameter]
        public Entity.Client Model { get; set; }
    }
}
