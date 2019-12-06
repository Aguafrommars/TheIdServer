using Aguacongas.IdentityServer.Store;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="GenericApiController{TUser}" />
    public class IdentityUserController<TUser> : GenericApiController<TUser> where TUser: class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserController{TUser}"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public IdentityUserController(IAdminStore<TUser> store): base(store)
        {
        }
    }
}
