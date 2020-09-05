// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Indenties
    {
        protected override string SelectProperties => $"{nameof(IdentityResource.Id)},{nameof(IdentityResource.DisplayName)},{nameof(IdentityResource.Description)}";
        protected override string Expand => nameof(IdentityResource.Resources);
        protected override string ExportExpand => $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}";

    }
}
