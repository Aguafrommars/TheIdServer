// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class ExternalProviders
    {
        protected override string SelectProperties => $"{nameof(Entity.ExternalProvider.Id)},{nameof(Entity.ExternalProvider.DisplayName)},{nameof(Entity.ExternalProvider.KindName)}";

        protected override string ExportExpand => $"{nameof(Models.ExternalProvider.ClaimTransformations)}";
    }
}
