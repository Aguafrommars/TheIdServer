using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Roles
    {
        protected override string SelectProperties => $"{nameof(Entity.Role.Id)},{nameof(Entity.Role.Name)}";

        protected override string ExportExpand => nameof(Entity.Role.RoleClaims);
    }
}
