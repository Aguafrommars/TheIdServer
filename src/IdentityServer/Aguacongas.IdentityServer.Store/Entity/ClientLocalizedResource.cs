namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Client localized resource
    /// </summary>
    /// <seealso cref="LocalizedResourceBase" />
    /// <seealso cref="IClientSubEntity" />
    public class ClientLocalizedResource : LocalizedResourceBase, IClientSubEntity
    {
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public virtual Client Client { get; set; }
    }
}
