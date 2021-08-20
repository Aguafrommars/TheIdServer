using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Role API controller
    /// </summary>
    public class RoleController : GenericApiController<Role>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="manager"></param>
        public RoleController(IAdminStore<Role> store, RoleManager<IdentityRole> manager) :
            base(new CheckIdentityRulesRoleStore<IAdminStore<Role>>(store, manager))
        {
        }
    }
}
