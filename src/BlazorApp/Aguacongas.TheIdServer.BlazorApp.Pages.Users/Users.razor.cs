// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;

using System.Collections.Generic;
using System.Linq;

using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Users
{
    public partial class Users
    {
        protected override string SelectProperties => $"{nameof(Entity.User.Id)},{nameof(Entity.User.UserName)}";

        protected override string ExportExpand => $"{nameof(Entity.User.UserClaims)},{nameof(Entity.User.UserRoles)}";
    }
}
