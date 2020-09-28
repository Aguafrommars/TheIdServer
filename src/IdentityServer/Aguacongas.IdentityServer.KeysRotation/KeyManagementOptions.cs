using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Options that control how an <see cref="Microsoft.AspNetCore.DataProtection.KeyManagement.IKeyManager"/> should behave.
    /// </summary>
    public class KeyManagementOptions : Microsoft.AspNetCore.DataProtection.KeyManagement.KeyManagementOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManagementOptions"/> class.
        /// </summary>
        public KeyManagementOptions():base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManagementOptions"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        internal KeyManagementOptions(KeyManagementOptions other)
        {
            if (other == null)
            {
                return;
            }

            KeyPropagationWindow = other.KeyPropagationWindow;
            MaxServerClockSkew = other.MaxServerClockSkew;
            KeyRingRefreshPeriod = other.KeyRingRefreshPeriod;
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
        /// Specifies the period before key expiration in which a new key should be generated
        /// so that it has time to propagate fully throughout the key ring. For example, if this
        /// period is 72 hours, then a new key will be created and persisted to storage
        /// approximately 72 hours before expiration.
        /// </summary>
        /// <remarks>
        /// The default value is currently fixed at 2 weeks.
        /// </remarks>
        public TimeSpan KeyPropagationWindow { get; set; } = TimeSpan.FromDays(14);

        /// <summary>
        /// Specifies the maximum clock skew allowed between servers when reading
        /// keys from the key ring. The key ring may use a key which has not yet
        /// been activated or which has expired if the key's valid lifetime is within
        /// the allowed clock skew window. This value can be set to <see cref="TimeSpan.Zero"/>
        /// if key activation and expiration times should be strictly honored by this server.
        /// </summary>
        /// <remarks>
        /// The default value value is currently fixed at 5 minutes.
        /// </remarks>
        public TimeSpan MaxServerClockSkew { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Controls the auto-refresh period where the key ring provider will
        /// flush its collection of cached keys and reread the collection from
        /// backing storage.
        /// </summary>
        /// <remarks>
        /// The default value is currently fixed at 24 hours.
        /// </remarks>
        public TimeSpan KeyRingRefreshPeriod { get; set; } = TimeSpan.FromHours(24);
    }
}
