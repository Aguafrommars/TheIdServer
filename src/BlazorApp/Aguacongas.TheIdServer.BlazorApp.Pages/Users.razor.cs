// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Users
    {
        protected override string SelectProperties => $"{nameof(Entity.User.Id)},{nameof(Entity.User.UserName)}";

        protected override string ExportExpand => $"{nameof(Entity.User.UserClaims)},{nameof(Entity.User.UserRoles)}";
    }
}
