namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Entity resource interface
    /// </summary>
    public interface IEntityResource
    {
        /// <summary>
        /// Gets or sets the kind of the resource.
        /// </summary>
        /// <value>
        /// The kind of the resource.
        /// </value>
        EntityResourceKind ResourceKind { get; set; }
    }
}
