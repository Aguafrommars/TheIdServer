// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;
#else
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;
#endif
using Microsoft.IdentityModel.Tokens;
using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal class ECDsaEncryptor : ISigningAlgortithmEncryptor
    {
        private readonly ECDsaSecurityKey _keyDerivationKey;

        public ECDsaEncryptor(ECDsaSecurityKey keyDerivationKey)
        {
            _keyDerivationKey = keyDerivationKey;
        }

        public SecurityKeyInfo GetSecurityKeyInfo(string signingAlgorithm)
        => GetSecurityKeyInfo(Enum.Parse<ECDsaSigningAlgorithm>(signingAlgorithm));
        public SigningCredentials GetSigningCredentials(string signingAlgorithm)
        => GetSigningCredentials(Enum.Parse<ECDsaSigningAlgorithm>(signingAlgorithm));

        public SigningCredentials GetSigningCredentials(ECDsaSigningAlgorithm rsaSigningAlgorithm)
        => new(_keyDerivationKey, rsaSigningAlgorithm.ToString());        

        public SecurityKeyInfo GetSecurityKeyInfo(ECDsaSigningAlgorithm rsaSigningAlgorithm)
        => new()
            {
                Key = _keyDerivationKey,
                SigningAlgorithm = rsaSigningAlgorithm.ToString()
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