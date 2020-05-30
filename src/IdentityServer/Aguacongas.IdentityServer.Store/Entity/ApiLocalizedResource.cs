namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Api localized resource
    /// </summary>
    /// <seealso cref="LocalizedResourceBase" />
    /// <seealso cref="IApiSubEntity" />
    public class ApiLocalizedResource : LocalizedResourceBase, IApiSubEntity
    {
        /// <summary>
        /// Gets or sets the API identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        public string ApiId { get; set; }

        /// <summary>
        /// Gets or sets the API.
        /// </summary>
        /// <value>
        /// The API.
        /// </value>
        public virtual ProtectResource Api { get; set; }
    }
}
