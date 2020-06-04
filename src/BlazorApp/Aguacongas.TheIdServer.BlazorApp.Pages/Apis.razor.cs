using Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Apis
    {
        protected override string SelectProperties => $"{nameof(ProtectResource.Id)},{nameof(ProtectResource.DisplayName)},{nameof(ProtectResource.Description)}";

        protected override string Expand => nameof(ProtectResource.Resources);
    }
}
