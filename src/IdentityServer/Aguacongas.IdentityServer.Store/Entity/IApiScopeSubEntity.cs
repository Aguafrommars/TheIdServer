namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Api sub entity interface
    /// </summary>
    public interface IApiScopeSubEntity
    {
        /// <summary>
        /// Gets or sets the API identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        string ApiScopeId { get; set; }
    }
}
