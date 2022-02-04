// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Apis
{
    public partial class Apis
    {
        protected override string SelectProperties => $"{nameof(ProtectResource.Id)},{nameof(ProtectResource.DisplayName)},{nameof(ProtectResource.Description)}";

        protected override string Expand => nameof(ProtectResource.Resources);

        protected override string ExportExpand => $"{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}";
    }
}
