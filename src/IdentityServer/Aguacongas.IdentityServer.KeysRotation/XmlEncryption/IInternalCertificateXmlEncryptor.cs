// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/DataProtection/src/XmlEncryption/IInternalCertificateXmlEncryptor.cs
// with namespace change from proginal Microsoft.AspNetCore.DataProtection.XmlEncryption

using System.Security.Cryptography.Xml;
using System.Xml;

// namespace change from proginal Microsoft.AspNetCore.DataProtection.XmlEncryption
namespace Aguacongas.IdentityServer.KeysRotation.XmlEncryption
{
    /// <summary>
    /// Internal implementation details of <see cref="CertificateXmlEncryptor"/> for unit testing.
    /// </summary>
    internal interface IInternalCertificateXmlEncryptor
    {
        EncryptedData PerformEncryption(EncryptedXml encryptedXml, XmlElement elementToEncrypt);
    }
}
