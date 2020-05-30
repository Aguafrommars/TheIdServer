namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Identity localized resource
    /// </summary>
    /// <seealso cref="LocalizedResourceBase" />
    /// <seealso cref="IIdentitySubEntity" />
    public class IdentityLocalizedResource : LocalizedResourceBase, IIdentitySubEntity
    {
        /// <summary>
        /// Gets or sets the identity resource identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        public string IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        public virtual IdentityResource Identity { get; set; }
    }
}
