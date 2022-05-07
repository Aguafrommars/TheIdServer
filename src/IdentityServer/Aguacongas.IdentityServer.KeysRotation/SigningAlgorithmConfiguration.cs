using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public class SigningAlgorithmConfiguration : AlgorithmConfiguration
    {
        public virtual Type EncryptionAlgorithmType { get; set; }

        public virtual string SigningAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the size of the encryption algorithm key.
        /// </summary>
        /// <value>
        /// The size of the encryption algorithm key.
        /// </value>
        public virtual int EncryptionAlgorithmKeySize { get; set; } = 2048;

        /// <summary>
        /// Gets or sets the size of the key identifier.
        /// </summary>
        /// <value>
        /// The size of the key identifier.
        /// </value>
        public virtual int KeyIdSize { get; set; } = 128;

        /// <summary>
        /// Gets or sets the key retirement.
        /// </summary>
        /// <value>
        /// The key retirement.
        /// </value>
        public TimeSpan KeyRetirement { get; set; } = TimeSpan.FromDays(180);

        /// <summary>
        /// When true, the signing algorithm is the default
        /// </summary>
        public bool IsDefaultSigningAlgorithm { get; set; }

        public override IAuthenticatedEncryptorDescriptor CreateNewDescriptor()
        {
            throw new NotImplementedException();
        }
    }
}
