namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba;

/// <summary>
/// Represents the view model for the CIBA (Client-Initiated Backchannel Authentication) consent screen.
/// </summary>
public class ViewModel
{
    /// <summary>
    /// Gets or sets the identifier for the CIBA request.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the client requesting authentication.
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// Gets or sets the URL of the client application.
    /// </summary>
    public string? ClientUrl { get; set; }

    /// <summary>
    /// Gets or sets the logo URL of the client application.
    /// </summary>
    public string? ClientLogoUrl { get; set; }

    /// <summary>
    /// Gets or sets the binding message associated with the authentication request.
    /// </summary>
    public string? BindingMessage { get; set; }

    /// <summary>
    /// Gets or sets the description of the authentication request.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the input model containing user consent data.
    /// </summary>
    public InputModel? Input { get; set; }

    /// <summary>
    /// Gets or sets the collection of identity scopes requested by the client.
    /// </summary>
    public IEnumerable<ScopeViewModel>? IdentityScopes { get; set; }

    /// <summary>
    /// Gets or sets the collection of API scopes requested by the client.
    /// </summary>
    public IEnumerable<ScopeViewModel>? ApiScopes { get; set; }
}

