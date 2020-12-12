// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.User.Components
{
    public partial class UserTokens
    {
        private IEnumerable<UserToken> Tokens => Collection.Where(t => t.LoginProvider.Contains(HandleModificationState.FilterTerm) || t.Name.Contains(HandleModificationState.FilterTerm) || t.Value.Contains(HandleModificationState.FilterTerm));
    }
}
