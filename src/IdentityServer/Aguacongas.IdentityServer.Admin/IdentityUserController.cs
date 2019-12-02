using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.IdentityServer.Admin
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GenericApiController{IdentityUser}" />
    public class IdentityUserController : GenericApiController<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserController"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public IdentityUserController(IAdminStore<IdentityUser> store): base(store)
        {

        }
    }
}
