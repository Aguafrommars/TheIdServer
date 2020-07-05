using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Import/export controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("[controller]")]
    public class ImportController : Controller
    {
        private readonly IAdminStore<OneTimeToken> _store;

        public ImportController(IAdminStore<OneTimeToken> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Creates the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [HttpPost()]
        [Authorize(Policy = "Is4-Writer")]
        public async Task<IActionResult> Import([FromBody] IFormFile file)
        {
            return Ok();
        }
            
    }
}
