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
        /// <param name="registration">The client registration.</param>
        /// <param name="uri">Base uri.</param>
        /// <returns></returns>
        Task<ClientRegisteration> RegisterAsync(ClientRegisteration registration, string uri);
    }
}