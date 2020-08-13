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
        /// <summary>
        /// Updates the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        Task<ClientRegisteration> UpdateRegistrationAsync(string clientId, ClientRegisteration registration, string uri);

        /// <summary>
        /// Gets the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        Task<ClientRegisteration> GetRegistrationAsync(string clientId, string uri);

        /// <summary>
        /// Deletes the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        Task DeleteRegistrationAsync(string clientId);
    }
}