// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IImportService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportController"/> class.
        /// </summary>
        /// <param name="serice">The serice.</param>
        /// <exception cref="ArgumentNullException">serice</exception>
        public ImportController(IImportService serice)
        {
            _service = serice ?? throw new ArgumentNullException(nameof(serice));
        }

        /// <summary>
        /// Imports files.
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Authorize(Policy = SharedConstants.WRITERPOLICY)]
        public Task<ImportResult> ImportAsync()
            => _service.ImportAsync(HttpContext.Request.Form.Files);
            
    }
}
