using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Admin
{
    [Route("client")]
    public class ClientController : ApiControllerBase<Client>
    {
        public ClientController(IAdminStore<Client> store):base(store)
        {
        }
    }
}
