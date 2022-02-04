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
    public sealed class RsaEncryptorConfiguration : AlgorithmConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the encryption algorithm.
        /// </summary>
        /// <value>
        /// The type of the encryption algorithm.
        /// </value>
        public Type EncryptionAlgorithmType { get; set; } = typeof(RSA);

        /// <summary>
        /// Gets or sets the size of the encryption algorithm key.
        /// </summary>
        /// <value>
        /// The size of the encryption algorithm key.
        /// </value>
        public int EncryptionAlgorithmKeySize { get; set; } = 2048;

        /// <summary>
        /// Gets or sets the RSA signing algorithm.
        /// </summary>
        /// <value>
        /// The RSA signing algorithm.
        /// </value>
        public RsaSigningAlgorithm RsaSigningAlgorithm { get; set; } = RsaSigningAlgorithm.RS256;

        /// <summary>
        /// Gets or sets the size of the key identifier.
        /// </summary>
        /// <value>
        /// The size of the key identifier.
        /// </value>
        public int KeyIdSize { get; set; } = 128;

        /// <summary>
        /// Gets or sets the key retirement.
        /// </summary>
        /// <value>
        /// The key retirement.
        /// </value>
        public TimeSpan KeyRetirement { get; set; } = TimeSpan.FromDays(180);

        public override IAuthenticatedEncryptorDescriptor CreateNewDescriptor()
        {
            return new RsaEncryptorDescriptor(this);
        }

    }
}
