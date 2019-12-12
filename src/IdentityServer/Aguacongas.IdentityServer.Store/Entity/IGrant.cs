using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Class implemting this intercace are grant
    /// </summary>
    /// <seealso cref="IAuditable" />
    public interface IGrant : IAuditable
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        string ClientId { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        string Data { get; set; }
        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        string UserId { get; set; }
    }
}