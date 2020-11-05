// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Identities
{
    public partial class Indenties
    {
        protected override string SelectProperties => $"{nameof(IdentityResource.Id)},{nameof(IdentityResource.DisplayName)}";
        protected override string Expand => nameof(IdentityResource.Resources);
        protected override string ExportExpand => $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}";

    }
}
