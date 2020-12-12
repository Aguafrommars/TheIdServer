// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.User.Components
{
    public partial class UserClaims
    {
        private IEnumerable<UserClaim> Claims => Collection.Where(c => c.Id == null || (c.ClaimType != null && c.ClaimType.Contains(HandleModificationState.FilterTerm)) || (c.ClaimValue != null && c.ClaimValue.Contains(HandleModificationState.FilterTerm)));
    
        private void OnDeleteClaimClicked(UserClaim claim)
        {
            Collection.Remove(claim);
            HandleModificationState.EntityDeleted(claim);
        }
    }
}
