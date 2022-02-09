// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParty.Components
{
    public partial class ClaimMappings
    {
        IEnumerable<RelyingPartyClaimMapping> Mappings => Collection.Where(c => c.FromClaimType == null || 
            c.FromClaimType.Contains(HandleModificationState.FilterTerm) || 
            (c.ToClaimType != null && 
                c.ToClaimType.Contains(HandleModificationState.FilterTerm)));
    }
}
