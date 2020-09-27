using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System;
using System.Security.Cryptography;
using static IdentityServer4.IdentityServerConstants;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class RsaEncryptorConfiguration : AlgorithmConfiguration
    {
        public Type EncryptionAlgorithmType { get; set; } = typeof(RSA);

        public int EncryptionAlgorithmKeySize { get; set; } = 2048;

        public RsaSigningAlgorithm RsaSigningAlgorithm { get; set; } = RsaSigningAlgorithm.RS256;

        public int KeyIdSize { get; set; } = 128;

        public override IAuthenticatedEncryptorDescriptor CreateNewDescriptor()
        {
            return new RsaEncryptorDescriptor(this);
        }

    }
}
