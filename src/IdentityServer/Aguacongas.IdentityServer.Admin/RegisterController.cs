using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private readonly IRegisterClientService _registerClientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterController"/> class.
        /// </summary>
        /// <param name="registerClientService">The register client service.</param>
        /// <exception cref="ArgumentNullException">registerClientService</exception>
        public RegisterController(IRegisterClientService registerClientService)
        {
            _registerClientService = registerClientService ?? throw new ArgumentNullException(nameof(registerClientService));
        }

        /// <summary>
        /// Creates the registration asynchronous.
        /// </summary>
        /// <param name="registeration">The registeration.</param>
        /// <returns></returns>
        [HttpPost]
        public Task<ClientRegisteration> CreateAsync([FromBody] ClientRegisteration registeration)
            => _registerClientService.RegisterAsync(registeration, HttpContext);

        /// <summary>
        /// Gets registration the asynchronous.
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns></returns>
        [HttpGet("{clientId}")]
        [Authorize(Policy = SharedConstants.REGISTRATION)]
        public Task<ClientRegisteration> GetAsync(string clientId)
            => _registerClientService.GetRegistrationAsync(clientId, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/");

        /// <summary>
        /// Updates registration the asynchronous.
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <param name="registeration">The registeration.</param>
        /// <returns></returns>
        [HttpPut("{clientId}")]
        [Authorize(Policy = SharedConstants.REGISTRATION)]
        public Task<ClientRegisteration> UpdateAsync(string clientId, [FromBody] ClientRegisteration registeration)
            => _registerClientService.UpdateRegistrationAsync(clientId, registeration, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/");

        /// <summary>
        /// Updates registration the asynchronous.
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns></returns>
        [HttpDelete("{clientId}")]
        [Authorize(Policy = SharedConstants.REGISTRATION)]
        public Task DeleteAsync(string clientId)
            => _registerClientService.DeleteRegistrationAsync(clientId);
    }
}
