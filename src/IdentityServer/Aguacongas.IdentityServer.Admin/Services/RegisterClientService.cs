using Aguacongas.IdentityServer.Admin.Models;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Aguacongas.IdentityServer.Admin.Services.IRegisterClientService" />
    public class RegisterClientService : IRegisterClientService
    {
        /// <summary>
        /// Registers the asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public async Task<ClientRegisteration> RegisterAsync(ClientRegisteration client)
        {
            return client;
        }
    }
}
