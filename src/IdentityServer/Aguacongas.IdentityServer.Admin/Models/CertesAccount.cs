using System;

namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// defines a Certes account
    /// </summary>
    public class CertesAccount
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CertesAccount"/> is enable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enable; otherwise, <c>false</c>.
        /// </value>
        public bool Enable { get; set; }
        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; } = "theidserver.com";
        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the account der.
        /// </summary>
        /// <value>
        /// The account der.
        /// </value>
        public string AccountDer { get; set; }


        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string PfxPassword { get; set; } = "theidserver.pwd";

        /// <summary>
        /// Gets the PFX path.
        /// </summary>
        /// <value>
        /// The PFX path.
        /// </value>
        public string PfxPath { get; set; } = "theidserver.pfx";

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}
