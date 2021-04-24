// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProviders
{
    public partial class ExternalProviders
    {
        protected override string SelectProperties => $"{nameof(Entity.ExternalProvider.Id)},{nameof(Entity.ExternalProvider.DisplayName)},{nameof(Entity.ExternalProvider.KindName)}";

        protected override string ExportExpand => $"{nameof(Models.ExternalProvider.ClaimTransformations)}";

        protected override string CreateRequestFilter(string filter)
        {
            return base.CreateRequestFilter(filter).Replace(nameof(Entity.ExternalProvider.KindName), nameof(Entity.ExternalProvider.SerializedHandlerType));
        }
    }
}
