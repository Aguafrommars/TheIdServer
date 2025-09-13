// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParties
{
    public partial class RelyingParties
    {
        protected override string SelectProperties => $"{nameof(Entity.RelyingParty.Id)},{nameof(Entity.RelyingParty.Description)}";
        protected override string Expand => null;

        protected override string ExportExpand => $"{nameof(Entity.RelyingParty.ClaimMappings)}";
    }
}
