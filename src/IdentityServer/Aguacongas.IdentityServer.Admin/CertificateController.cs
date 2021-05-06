﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// certificate controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("[controller]")]
    public class CertificateController : Controller
    {
        private readonly ICertificateVerifierService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException">service</exception>
        public CertificateController(ICertificateVerifierService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Verities the specified certificate content.
        /// </summary>
        /// <returns>
        /// The thumbprint or the error status
        /// </returns>
        [HttpPut]
        public Task<IEnumerable<string>> VerityAsync()
        => _service.VerifyAsync(Request.Body);
    }
}
