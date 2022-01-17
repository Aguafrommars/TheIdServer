using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    /// <summary>
    /// Class implementing this interface creates personal access token (PAT)
    /// </summary>
    public interface ICreatePersonalAccessToken
    {
        /// <summary>
        /// Create a personal access token for the current user and client
        /// </summary>
        /// <param name="context">The Http context</param>
        /// <param name="isRefenceToken">Creeate a reference token if true otherwise a JWT token</param>
        /// <param name="lifetimeDays">The token life time</param>
        /// <param name="apis">The list of audience</param>
        /// <param name="scopes">The list of scopes</param>
        /// <param name="claimTypes">The list of claims types</param>
        /// <returns></returns>
        Task<string> CreatePersonalAccessTokenAsync(HttpContext context, 
            bool isRefenceToken, 
            int lifetimeDays,
            IEnumerable<string> apis, 
            IEnumerable<string> scopes, 
            IEnumerable<string> claimTypes);
    }
}
