// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public partial class ClaimTransformations
    {
        IEnumerable<ExternalClaimTransformation> Transformations => Collection.Where(t => t.FromClaimType == null ||
            t.FromClaimType.Contains(HandleModificationState.FilterTerm) ||
            (t.ToClaimType != null &&
                t.ToClaimType.Contains(HandleModificationState.FilterTerm)));
    }
}
