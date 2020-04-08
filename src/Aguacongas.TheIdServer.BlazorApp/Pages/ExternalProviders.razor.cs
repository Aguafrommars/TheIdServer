using Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class ExternalProviders
    {
        protected override string SelectProperties => $"{nameof(ExternalProvider.Id)},{nameof(ExternalProvider.DisplayName)}";
    }
}
