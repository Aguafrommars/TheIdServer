// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;
#else
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;
#endif
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal class RsaEncryptor: IAuthenticatedEncryptor
    {
        private readonly RsaSecurityKey _keyDerivationKey;

        public RsaEncryptor(RsaSecurityKey keyDerivationKey)
        {
            _keyDerivationKey = keyDerivationKey;        
        }

        public SigningCredentials GetSigningCredentials(RsaSigningAlgorithm rsaSigningAlgorithm)
        {
            return new SigningCredentials(_keyDerivationKey, rsaSigningAlgorithm.ToString());
        }

        public SecurityKeyInfo GetSecurityKeyInfo(RsaSigningAlgorithm rsaSigningAlgorithm)
        {
            return new SecurityKeyInfo
            {
                Key = _keyDerivationKey,
                SigningAlgorithm = rsaSigningAlgorithm.ToString()
            };
        }

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