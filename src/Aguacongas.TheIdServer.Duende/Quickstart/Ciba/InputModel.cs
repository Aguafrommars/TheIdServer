namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba;

/// <summary>
/// Represents the input model for CIBA (Client-Initiated Backchannel Authentication) consent.
/// </summary>
public class InputModel
{
    /// <summary>
    /// Gets or sets the button value indicating the user's action (e.g., "yes", "no").
    /// </summary>
    public string? Button { get; set; }

    /// <summary>
    /// Gets or sets the scopes that the user has consented to.
    /// </summary>
    public IEnumerable<string>? ScopesConsented { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the CIBA request.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the description provided by the user for the consent.
    /// </summary>
    public string? Description { get; set; }
}
