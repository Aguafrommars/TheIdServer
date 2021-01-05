// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/AzureKeyVault/src/IKeyVaultWrappingClient.cs
// with namespace change from original Microsoft.AspNetCore.DataProtection.AzureKeyVault
using Microsoft.Azure.KeyVault.Models;
using System.Threading.Tasks;

// namespace change from original Microsoft.AspNetCore.DataProtection.AzureKeyVault
namespace Aguacongas.IdentityServer.KeysRotation.AzureKeyVault
{
    public interface IKeyVaultWrappingClient
    {
        Task<KeyOperationResult> UnwrapKeyAsync(string keyIdentifier, string algorithm, byte[] cipherText);
        Task<KeyOperationResult> WrapKeyAsync(string keyIdentifier, string algorithm, byte[] cipherText);
    }
}