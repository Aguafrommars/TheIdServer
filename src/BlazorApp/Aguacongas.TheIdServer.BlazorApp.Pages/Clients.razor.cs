using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Clients
    {
        protected override string SelectProperties => $"{nameof(Entity.Client.Id)},{nameof(Entity.Client.ClientName)},{nameof(Entity.Client.Description)}";

        protected override string Expand => nameof(Entity.Client.Resources);
    }
}
