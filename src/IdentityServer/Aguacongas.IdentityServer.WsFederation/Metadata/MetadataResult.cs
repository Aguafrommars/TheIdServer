// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IActionResult" />
    public class MetadataResult : IActionResult
    {
        private readonly WsFederationConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataResult"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public MetadataResult(WsFederationConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
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
        public Task ExecuteResultAsync(ActionContext context)
        {
            var ser = new WsFederationMetadataSerializer();
            using var ms = new MemoryStream();
            using XmlWriter writer = new RoleSupportedClaimsWriter(XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false), _config);
            ser.WriteMetadata(writer, _config);
            writer.Flush();
            context.HttpContext.Response.ContentType = "application/xml";
            var metaAsString = Encoding.UTF8.GetString(ms.ToArray());
            return context.HttpContext.Response.WriteAsync(metaAsString);
        }
    }
}
