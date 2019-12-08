using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// User Idp login
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        /// <value>
        /// The login provider.
        /// </value>
        [Key]
        public string LoginProvider { get; set; }
        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        /// <value>
        /// The provider key.
        /// </value>
        [Key]
        public string ProviderKey { get; set; }
        /// <summary>
        /// Gets or sets the display name of the provider.
        /// </summary>
        /// <value>
        /// The display name of the provider.
        /// </value>
        public string ProviderDisplayName { get; set; }
    }
}
