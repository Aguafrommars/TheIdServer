// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
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
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns></returns>
        Task<ClientRegisteration> RegisterAsync(ClientRegisteration registration, HttpContext httpContext);
        /// <summary>
        /// Updates the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ClientRegisteration> UpdateRegistrationAsync(string clientId, ClientRegisteration registration, string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ClientRegisteration> GetRegistrationAsync(string clientId, string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the registration asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteRegistrationAsync(string clientId, CancellationToken cancellationToken);
    }
}