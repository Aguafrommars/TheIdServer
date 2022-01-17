using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class KeyManagementOptions
    {
        /// <summary>
        /// Specifies whether the data protection system should auto-generate keys.
        /// </summary>
        public bool AutoGenerateKeys { get; set; } = true;

        /// <summary>
        /// Controls the lifetime (number of days before expiration) for newly-generated keys.
        /// </summary>
        public TimeSpan NewKeyLifetime { get; set; } = TimeSpan.FromDays(90.0);

    }
}