﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2020 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/AzureKeyVault/src/AzureKeyVaultXmlDecryptor.cs
// with namespace change from original Microsoft.AspNetCore.DataProtection.AzureKeyVault
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;

// namespace change from original Microsoft.AspNetCore.DataProtection.AzureKeyVault
namespace Aguacongas.IdentityServer.KeysRotation.AzureKeyVault
{
    internal class AzureKeyVaultXmlDecryptor : IXmlDecryptor
    {
        private readonly IKeyVaultWrappingClient _client;

        public AzureKeyVaultXmlDecryptor(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetService<IKeyVaultWrappingClient>();
        }

        public XElement Decrypt(XElement encryptedElement)
        {
            return DecryptAsync(encryptedElement).GetAwaiter().GetResult();
        }

        private async Task<XElement> DecryptAsync(XElement encryptedElement)
        {
            var kid = (string)encryptedElement.Element("kid");
            var symmetricKey = Convert.FromBase64String((string)encryptedElement.Element("key"));
            var symmetricIV = Convert.FromBase64String((string)encryptedElement.Element("iv"));

            var encryptedValue = Convert.FromBase64String((string)encryptedElement.Element("value"));

            var result = await _client.UnwrapKeyAsync(kid, AzureKeyVaultXmlEncryptor.DefaultKeyEncryption, symmetricKey).ConfigureAwait(false);

            byte[] decryptedValue;
            using (var symmetricAlgorithm = AzureKeyVaultXmlEncryptor.DefaultSymmetricAlgorithmFactory())
            {
                using var decryptor = symmetricAlgorithm.CreateDecryptor(result.Result, symmetricIV);
                decryptedValue = decryptor.TransformFinalBlock(encryptedValue, 0, encryptedValue.Length);
            }

            using var memoryStream = new MemoryStream(decryptedValue);
            return XElement.Load(memoryStream);
        }
    }
}
