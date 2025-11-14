// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre
// Migration to Azure.Security.KeyVault 2025

using Azure.Core;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.KeysRotation.AzureKeyVault
{
    public class KeyVaultClientWrapper : IKeyVaultWrappingClient
    {
        private readonly KeyClient _keyClient;
        private readonly TokenCredential _credential;

        /// <summary>
        /// Creates a new instance of KeyVaultClientWrapper using the specified KeyClient
        /// </summary>
        /// <param name="keyClient">The KeyClient to use for key operations</param>
        /// <param name="credential">The TokenCredential for authentication</param>
        public KeyVaultClientWrapper(KeyClient keyClient, TokenCredential credential)
        {
            _keyClient = keyClient ?? throw new ArgumentNullException(nameof(keyClient));
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        }

        /// <summary>
        /// Creates a new instance of KeyVaultClientWrapper using a vault URI and credential
        /// </summary>
        /// <param name="vaultUri">The URI of the Key Vault</param>
        /// <param name="credential">The TokenCredential for authentication</param>
        public KeyVaultClientWrapper(Uri vaultUri, TokenCredential credential)
        {
            ArgumentNullException.ThrowIfNull(vaultUri);

            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
            _keyClient = new KeyClient(vaultUri, credential);
        }

        public async Task<UnwrapResult> UnwrapKeyAsync(string keyIdentifier, KeyWrapAlgorithm algorithm, byte[] encryptedKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(keyIdentifier))
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }

            var cryptoClient = GetCryptographyClient(keyIdentifier);
            return await cryptoClient.UnwrapKeyAsync(algorithm, encryptedKey, cancellationToken).ConfigureAwait(false);
        }

        public async Task<WrapResult> WrapKeyAsync(string keyIdentifier, KeyWrapAlgorithm algorithm, byte[] key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(keyIdentifier))
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }

            var cryptoClient = GetCryptographyClient(keyIdentifier);
            return await cryptoClient.WrapKeyAsync(algorithm, key, cancellationToken).ConfigureAwait(false);
        }

        private CryptographyClient GetCryptographyClient(string keyIdentifier)
        {
            // keyIdentifier can be either:
            // 1. A full key ID: https://{vault}.vault.azure.net/keys/{key-name}/{version}
            // 2. A key name (in which case we use the KeyClient's vault)

            if (Uri.TryCreate(keyIdentifier, UriKind.Absolute, out var keyUri))
            {
                // Full URI provided
                return new CryptographyClient(keyUri, _credential);
            }
            else
            {
                // Key name only, get the key from KeyClient
                var key = _keyClient.GetKey(keyIdentifier);
                return new CryptographyClient(key.Value.Id, _credential);
            }
        }
    }
}