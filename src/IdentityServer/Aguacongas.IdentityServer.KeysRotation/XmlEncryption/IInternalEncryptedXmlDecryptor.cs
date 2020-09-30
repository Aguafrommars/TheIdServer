// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/XmlEncryption/IInternalEncryptedXmlDecryptor.cs
// but adapted for our needs

using System.Security.Cryptography.Xml;

namespace Aguacongas.IdentityServer.KeysRotation.XmlEncryption
{
    public interface IInternalEncryptedXmlDecryptor
    {
        void PerformPreDecryptionSetup(EncryptedXml encryptedXml);
    }
}