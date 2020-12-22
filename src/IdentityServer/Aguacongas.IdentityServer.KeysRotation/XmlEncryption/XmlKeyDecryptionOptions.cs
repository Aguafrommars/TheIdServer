// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2020 @Olivier Lefebvre

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/XmlEncryption/XmlKeyDecryptionOptions.cs
// with namespace change from proginal Microsoft.AspNetCore.DataProtection.XmlEncryption

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.KeysRotation.XmlEncryption
{
    /// <summary>
    /// Specifies settings for how to decrypt XML keys.
    /// </summary>
    internal class XmlKeyDecryptionOptions
    {
        private readonly Dictionary<string, List<X509Certificate2>> _certs = new Dictionary<string, List<X509Certificate2>>(StringComparer.Ordinal);

        public int KeyDecryptionCertificateCount => _certs.Count;

        public bool TryGetKeyDecryptionCertificates(X509Certificate2 certInfo, out IReadOnlyList<X509Certificate2> keyDecryptionCerts)
        {
            var key = GetKey(certInfo);
            var retVal = _certs.TryGetValue(key, out var keyDecryptionCertsRetVal);
            keyDecryptionCerts = keyDecryptionCertsRetVal;
            return retVal;
        }

        public void AddKeyDecryptionCertificate(X509Certificate2 certificate)
        {
            var key = GetKey(certificate);
            if (!_certs.TryGetValue(key, out var certificates))
            {
                certificates = _certs[key] = new List<X509Certificate2>();
            }
            certificates.Add(certificate);
        }

        private static string GetKey(X509Certificate2 cert) => cert.Thumbprint;
    }
}
