// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Aguacongas.IdentityServer.WsFederation.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IActionResult" />
    public class MetadataResult : IActionResult
    {
        private readonly WsFederationConfiguration _config;
        private readonly IMetatdataSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataResult"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="serializer">The metatdata serializer</param>
        public MetadataResult(WsFederationConfiguration config, IMetatdataSerializer serializer)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Executes the result operation of the action method asynchronously. This method is called by MVC to process
        /// the result of an action method.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes
        /// information about the action that was executed and request information.</param>
        /// <returns>
        /// A task that represents the asynchronous execute operation.
        /// </returns>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var metaAsString = await _serializer.SerializeAsync(_config).ConfigureAwait(false);
            context.HttpContext.Response.ContentType = "application/xml";
            await context.HttpContext.Response.WriteAsync(metaAsString).ConfigureAwait(false);
        }
    }
}
