// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/DataProtection/src/XmlEncryption/IInternalEncryptedXmlDecryptor.cs
// with namespace change from proginal Microsoft.AspNetCore.DataProtection.XmlEncryption

using System.Security.Cryptography.Xml;

// namespace change from proginal Microsoft.AspNetCore.DataProtection.XmlEncryption
namespace Aguacongas.IdentityServer.KeysRotation.XmlEncryption
{
    /// <summary>
    /// Internal implementation details of <see cref="EncryptedXmlDecryptor"/> for unit testing.
    /// </summary>
    public interface IInternalEncryptedXmlDecryptor
    {
        void PerformPreDecryptionSetup(EncryptedXml encryptedXml);
    }
}