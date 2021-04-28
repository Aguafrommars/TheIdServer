// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// Ws-Federation endpoints controller
    /// </summary>
    [Route("[controller]")]
    public class WsFederationController : Controller
    {
        private readonly IMetadataResponseGenerator _metadata;
        private readonly IWsFederationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="WsFederationController"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException">
        /// metadata
        /// or
        /// service
        /// </exception>
        public WsFederationController(IMetadataResponseGenerator metadata, IWsFederationService service)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata")]
        public async Task<IActionResult> Metadata()
        => new MetadataResult(await _metadata.GenerateAsync(Url.Action(nameof(Index), "WsFederation", null, Request.Scheme, Request.Host.Value)).ConfigureAwait(false));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<IActionResult> Index()
        => _service.ProcessRequest(Request, Url);
    }
}
