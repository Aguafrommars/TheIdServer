// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System;
using System.Security.Cryptography;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class RsaEncryptorConfiguration : SigningAlgorithmConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the encryption algorithm.
        /// </summary>
        /// <value>
        /// The type of the encryption algorithm.
        /// </value>
        public override Type EncryptionAlgorithmType { get; set; } = typeof(RSA);

        /// <summary>
        /// Gets or sets the RSA signing algorithm.
        /// </summary>
        /// <value>
        /// The RSA signing algorithm.
        /// </value>
        public override string SigningAlgorithm { get; set; } = RsaSigningAlgorithm.RS256.ToString();


        public override IAuthenticatedEncryptorDescriptor CreateNewDescriptor()
        {
            return new RsaEncryptorDescriptor(this);
        }
    }
}
