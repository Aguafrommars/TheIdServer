using System;

namespace Aguacongas.IdentityServer.Store.Entity;

/// <summary>
/// Represents a persisted Pushed Authorization Request.
/// </summary>
public class PushedAuthorizationRequest : IEntityId, IAuditable
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    /// <remarks>
    /// This is the hash
    /// </remarks>
    public string Id { get; set; }

    /// <summary>
    /// Gets the Expires at
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Gets the request parameters.
    /// </summary>
    public string Parameters { get; set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; set; }
    
    /// <inheritdoc/>
    public DateTime? ModifiedAt { get; set; }
}
