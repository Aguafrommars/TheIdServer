namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Class implementing this interface is client sub entity
    /// </summary>
    public interface IClientSubEntity
    {
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        string ClientId { get; set; }
    }
}
