// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Clients
{
    public partial class Clients
    {
        protected override string SelectProperties => $"{nameof(Entity.Client.Id)},{nameof(Entity.Client.ClientName)},{nameof(Entity.Client.Description)}";

        protected override string Expand => nameof(Entity.Client.Resources);
        protected override string ExportExpand => $"{nameof(Entity.Client.IdentityProviderRestrictions)},{nameof(Entity.Client.ClientClaims)},{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.AllowedGrantTypes)},{nameof(Entity.Client.RedirectUris)},{nameof(Entity.Client.AllowedScopes)},{nameof(Entity.Client.Properties)},{nameof(Entity.Client.Resources)}";
    }
}