namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Entity id interface
    /// </summary>
    public interface IEntityId
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; set; }
    }
}
