using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class ApiScopes
    {
        protected override string SelectProperties => $"{nameof(Entity.ApiScope.Id)},{nameof(Entity.ApiScope.DisplayName)},{nameof(Entity.ApiScope.Description)}";
        protected override string Expand => nameof(Entity.ApiScope.Resources);
    }
}
