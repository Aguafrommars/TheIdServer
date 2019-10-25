using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public interface IGrant : IAuditable
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        string ClientId { get; set; }
        string Data { get; set; }
        string SubjectId { get; set; }
    }
}