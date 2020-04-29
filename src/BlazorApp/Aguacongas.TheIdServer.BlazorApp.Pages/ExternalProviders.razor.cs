using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class ExternalProviders
    {
        protected override string SelectProperties => $"{nameof(Entity.ExternalProvider.Id)},{nameof(Entity.ExternalProvider.DisplayName)},{nameof(Entity.ExternalProvider.KindName)}";
    }
}
