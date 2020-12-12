// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.User.Components
{
    public partial class UserLogins
    {
        private IEnumerable<UserLogin> Logins => Collection.Where(l => l.ProviderDisplayName.Contains(HandleModificationState.FilterTerm));
    }
}
