// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System;
using System.Security.Cryptography;
#if DUENDE
using static Duende.IdentityServer.IdentityServerConstants;
#else
using static IdentityServer4.IdentityServerConstants;
#endif

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class ECDsaEncryptorConfiguration : SigningAlgorithmConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the encryption algorithm.
        /// </summary>
        /// <value>
        /// The type of the encryption algorithm.
        /// </value>
        public override Type EncryptionAlgorithmType { get; set; } = typeof(ECDsa);

        public override int EncryptionAlgorithmKeySize { get; set; } = 521;

        /// <summary>
        /// Gets or sets the ECDsa signing algorithm.
        /// </summary>
        /// <value>
        /// The ECDsa signing algorithm.
        /// </value>
        public override string SigningAlgorithm { get; set; } = ECDsaSigningAlgorithm.ES256.ToString();

        public override IAuthenticatedEncryptorDescriptor CreateNewDescriptor()
        {
            return new ECDsaEncryptorDescriptor(this);
        }

    }
}
