// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2020 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/AzureKeyVault/src/KeyVaultClientWrapper.cs
// with namespace change from original Microsoft.AspNetCore.DataProtection.AzureKeyVault
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;

// namespace change from original Microsoft.AspNetCore.DataProtection.AzureKeyVault
namespace Aguacongas.IdentityServer.KeysRotation.AzureKeyVault
{
    public class KeyVaultClientWrapper : IKeyVaultWrappingClient
    {
        private readonly KeyVaultClient _client;

        public KeyVaultClientWrapper(KeyVaultClient client)
        {
            _client = client;
        }

        public Task<KeyOperationResult> UnwrapKeyAsync(string keyIdentifier, string algorithm, byte[] cipherText)
        {
            return _client.UnwrapKeyAsync(keyIdentifier, algorithm, cipherText);
        }

        public Task<KeyOperationResult> WrapKeyAsync(string keyIdentifier, string algorithm, byte[] cipherText)
        {
            return _client.WrapKeyAsync(keyIdentifier, algorithm, cipherText);
        }
    }
}