// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre
// Migration to Azure.Security.KeyVault 2025

using Azure.Security.KeyVault.Keys.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.KeysRotation.AzureKeyVault
{
    public interface IKeyVaultWrappingClient
    {
        Task<UnwrapResult> UnwrapKeyAsync(string keyIdentifier, KeyWrapAlgorithm algorithm, byte[] encryptedKey, CancellationToken cancellationToken = default);
        Task<WrapResult> WrapKeyAsync(string keyIdentifier, KeyWrapAlgorithm algorithm, byte[] key, CancellationToken cancellationToken = default);
    }
}