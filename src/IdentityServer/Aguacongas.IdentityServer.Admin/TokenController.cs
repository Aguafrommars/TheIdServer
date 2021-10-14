using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Personal access token controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Produces("application/json")]
    [Route("[controller]")]
    [Authorize(SharedConstants.TOKENPOLICY)]
    public class TokenController : Controller
    {
        private readonly ICreatePersonalAccessToken _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        public TokenController(ICreatePersonalAccessToken service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Creates a Personnal Access Token (PAT) for the current user and calling app.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("pat")]
        public Task<string> CreatePersonalAccessTokenAsync([FromBody] CreatePersonalAccessToken request)
            => _service.CreatePersonalAccessTokenAsync(HttpContext, 
                request.IsReferenceToken, 
                request.LifetimeDays, 
                request.Apis, 
                request.Scopes, 
                request.ClaimTypes);
    }
}
