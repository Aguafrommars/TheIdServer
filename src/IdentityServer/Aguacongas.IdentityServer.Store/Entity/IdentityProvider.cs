namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Represents a remote identity provider
    /// </summary>
    public class IdentityProvider
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }
    }
}
