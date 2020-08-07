using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Services;
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
    public class RegisterController
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
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "Is4-Writer")]
        public Task<ClientRegisteration> CreateAsync(ClientRegisteration client)
            => _registerClientService.RegisterAsync(client);
    }
}
