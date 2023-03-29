using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity;

/// <summary>
/// Sqml2P artifact
/// </summary>
public class Saml2pArtifact : IEntityId, IUserSubEntity, IClientSubEntity
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client.
    /// </value>
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the subject identifier.
    /// </summary>
    /// <value>
    /// The subject identifier.
    /// </value>
    [MaxLength(200)]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>
    /// The data.
    /// </value>
    public string Xml { get; set; }

    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    /// <value>
    /// The session identifier.
    /// </value>
    public string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    /// <value>
    /// The created at.
    /// </value>
    public DateTime CreatedAt { get; set; }
}
