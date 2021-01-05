// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ApiScopes
{
    public partial class ApiScopes
    {
        protected override string SelectProperties => $"{nameof(Entity.ApiScope.Id)},{nameof(Entity.ApiScope.DisplayName)},{nameof(Entity.ApiScope.Description)}";
        protected override string Expand => nameof(Entity.ApiScope.Resources);

        protected override string ExportExpand => $"{nameof(Entity.ApiScope.ApiScopeClaims)},{nameof(Entity.ApiScope.Properties)},{nameof(Entity.ApiScope.Resources)}";
    }
}
