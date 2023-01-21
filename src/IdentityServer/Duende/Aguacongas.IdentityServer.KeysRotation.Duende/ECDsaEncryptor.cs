// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Duende.IdentityServer.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal class ECDsaEncryptor : ISigningAlgortithmEncryptor
    {
        private readonly ECDsaSecurityKey _keyDerivationKey;

        public ECDsaEncryptor(ECDsaSecurityKey keyDerivationKey)
        {
            _keyDerivationKey = keyDerivationKey;
        }
        public SigningCredentials GetSigningCredentials(string signingAlgorithm)
        => GetSigningCredentials(Enum.Parse<ECDsaSigningAlgorithm>(signingAlgorithm));

        public SigningCredentials GetSigningCredentials(ECDsaSigningAlgorithm rsaSigningAlgorithm)
        => new(_keyDerivationKey, rsaSigningAlgorithm.ToString());        

        public SecurityKeyInfo GetSecurityKeyInfo(string signingAlgorithm)
        => GetSecurityKeyInfo(Enum.Parse<ECDsaSigningAlgorithm>(signingAlgorithm));
        public SecurityKeyInfo GetSecurityKeyInfo(ECDsaSigningAlgorithm ecdsaSigningAlgorithm)
        => new()
            {
                Key = _keyDerivationKey,
                SigningAlgorithm = ecdsaSigningAlgorithm.ToString()
            };

        public byte[] Decrypt(ArraySegment<byte> ciphertext, ArraySegment<byte> additionalAuthenticatedData)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(ArraySegment<byte> plaintext, ArraySegment<byte> additionalAuthenticatedData)
        {
            throw new NotImplementedException();
        }
    }
}