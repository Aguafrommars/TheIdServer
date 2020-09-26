using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public class KeyManagementOptions : Microsoft.AspNetCore.DataProtection.KeyManagement.KeyManagementOptions
    {
        private static readonly TimeSpan _keyRingRefreshPeriod = TimeSpan.FromHours(24);

        public KeyManagementOptions():base()
        { }

        internal KeyManagementOptions(KeyManagementOptions other)
        {
            if (other == null)
            {
                return;
            }

            AutoGenerateKeys = other.AutoGenerateKeys;
            NewKeyLifetime = other.NewKeyLifetime;
            XmlEncryptor = other.XmlEncryptor;
            XmlRepository = other.XmlRepository;
            AuthenticatedEncryptorConfiguration = other.AuthenticatedEncryptorConfiguration;

            foreach (var keyEscrowSink in other.KeyEscrowSinks)
            {
                KeyEscrowSinks.Add(keyEscrowSink);
            }

            foreach (var encryptorFactory in other.AuthenticatedEncryptorFactories)
            {
                AuthenticatedEncryptorFactories.Add(encryptorFactory);
            }
            
        }

        /// <summary>
        /// Controls the auto-refresh period where the key ring provider will
        /// flush its collection of cached keys and reread the collection from
        /// backing storage.
        /// </summary>
        /// <remarks>
        /// This value is currently fixed at 24 hours.
        /// </remarks>
        internal TimeSpan KeyRingRefreshPeriod
        {
            get
            {
                // This value is not settable since there's a complex interaction between
                // it and the key expiration safety period.
                return _keyRingRefreshPeriod;
            }
        }
    }
}
