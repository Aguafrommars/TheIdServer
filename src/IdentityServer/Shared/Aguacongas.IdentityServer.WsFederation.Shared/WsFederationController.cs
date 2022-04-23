// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Metadata;
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
        private readonly IMetatdataSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WsFederationController"/> class.
        /// </summary>
        /// <param name="metadata">The metadata response generator.</param>
        /// <param name="service">The service.</param>
        /// <param name="serializer">The WS-Federation metadata serializer</param>
        /// <exception cref="ArgumentNullException">
        /// metadata
        /// or
        /// service
        /// </exception>
        public WsFederationController(IMetadataResponseGenerator metadata, IWsFederationService service, IMetatdataSerializer serializer)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("metadata")]
        public async Task<IActionResult> Metadata()
        => new MetadataResult(await _metadata.GenerateAsync(Url.Action(nameof(Index), "WsFederation", null, Request.Scheme, Request.Host.Value)).ConfigureAwait(false), _serializer);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<IActionResult> Index()
        => _service.ProcessRequestAsync(Request, Url);
    }
}
