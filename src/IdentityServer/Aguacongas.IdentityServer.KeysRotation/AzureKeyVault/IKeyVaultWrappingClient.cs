// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Azure.KeyVault.Models;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.KeysRotation.AzureKeyVault
{
    public interface IKeyVaultWrappingClient
    {
        Task<KeyOperationResult> UnwrapKeyAsync(string keyIdentifier, string algorithm, byte[] cipherText);
        Task<KeyOperationResult> WrapKeyAsync(string keyIdentifier, string algorithm, byte[] cipherText);
    }
}