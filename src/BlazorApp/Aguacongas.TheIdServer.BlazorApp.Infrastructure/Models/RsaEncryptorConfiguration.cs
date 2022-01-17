using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public enum RsaSigningAlgorithm
    {
        RS256,
        RS384,
        RS512,
        PS256,
        PS384,
        PS512
    }

    public class RsaEncryptorConfiguration
    {
        /// <summary>
        /// Gets or sets the size of the encryption algorithm key.
        /// </summary>
        /// <value>
        /// The size of the encryption algorithm key.
        /// </value>
        public int EncryptionAlgorithmKeySize { get; set; }

        /// <summary>
        /// Gets or sets the RSA signing algorithm.
        /// </summary>
        /// <value>
        /// The RSA signing algorithm.
        /// </value>
        public RsaSigningAlgorithm RsaSigningAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the size of the key identifier.
        /// </summary>
        /// <value>
        /// The size of the key identifier.
        /// </value>
        public int KeyIdSize { get; set; }

        /// <summary>
        /// Gets or sets the key retirement.
        /// </summary>
        /// <value>
        /// The key retirement.
        /// </value>
        public TimeSpan KeyRetirement { get; set; }

    }
}