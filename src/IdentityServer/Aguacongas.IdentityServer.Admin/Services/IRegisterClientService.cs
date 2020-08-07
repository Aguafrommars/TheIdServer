using Aguacongas.IdentityServer.Admin.Models;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRegisterClientService
    {
        /// <summary>
        /// Registers the asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        Task<ClientRegisteration> RegisterAsync(ClientRegisteration client);
    }
}