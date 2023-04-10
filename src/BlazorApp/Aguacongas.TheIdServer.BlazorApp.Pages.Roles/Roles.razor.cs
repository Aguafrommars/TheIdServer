// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Roles
{
    public partial class Roles
    {
        protected override string SelectProperties => $"{nameof(Entity.Role.Id)},{nameof(Entity.Role.Name)},{nameof(Entity.Role.ConcurrencyStamp)}";

        protected override string ExportExpand => nameof(Entity.Role.RoleClaims);
    }
}
