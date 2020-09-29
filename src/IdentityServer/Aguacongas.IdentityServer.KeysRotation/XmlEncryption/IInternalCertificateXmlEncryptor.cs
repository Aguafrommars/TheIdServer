// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/XmlEncryption/IInternalCertificateXmlEncryptor.cs
// but adapted for our needs

using System.Security.Cryptography.Xml;
using System.Xml;

namespace Aguacongas.IdentityServer.KeysRotation.XmlEncryption
{
    internal interface IInternalCertificateXmlEncryptor
    {
        EncryptedData PerformEncryption(EncryptedXml encryptedXml, XmlElement elementToEncrypt);
    }
}
