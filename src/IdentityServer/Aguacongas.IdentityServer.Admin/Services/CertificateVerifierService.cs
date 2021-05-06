﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CertificateVerifierService : ICertificateVerifierService
    {
        /// <summary>
        /// Verifies the specified certificate content.
        /// </summary>
        /// <param name="certificateContent">Content of the certificate.</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> VerifyAsync(Stream certificateContent)
        {
            using var ms = new MemoryStream();
            await certificateContent.CopyToAsync(ms).ConfigureAwait(false);
            var certificate = new X509Certificate2(ms.ToArray());
            if (!certificate.Verify())
            {
                using var chain = new X509Chain();
                chain.Build(certificate);
                return chain.ChainStatus.Select(s => s.StatusInformation);
            }
            return new[] { certificate.Thumbprint };
        }
    }
}
