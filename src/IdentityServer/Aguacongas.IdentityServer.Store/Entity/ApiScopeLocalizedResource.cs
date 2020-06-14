namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Api scope localized resource
    /// </summary>
    /// <seealso cref="LocalizedResourceBase" />
    /// <seealso cref="IEntityResource" />
    public class ApiScopeLocalizedResource : LocalizedResourceBase, IEntityResource
    {
        /// <summary>
        /// Gets or sets the API identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        public string ApiScopeId { get; set; }

        /// <summary>
        /// Gets or sets the kind of the resource.
        /// </summary>
        /// <value>
        /// The kind of the resource.
        /// </value>
        public EntityResourceKind ResourceKind { get; set; }

        /// <summary>
        /// Gets or sets the API scope.
        /// </summary>
        /// <value>
        /// The API scope.
        /// </value>
        public virtual ApiScope ApiScope { get; set; }

    }
}
