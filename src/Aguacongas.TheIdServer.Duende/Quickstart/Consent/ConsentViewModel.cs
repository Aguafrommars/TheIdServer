// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.Duende.Quickstart.Consent;

/// <summary>
/// View model for the consent screen, containing client and scope information.
/// </summary>
public class ConsentViewModel : ConsentInputModel
{
    /// <summary>
    /// Gets or sets the name of the client requesting consent.
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
    /// Gets or sets the URL to the client's privacy policy.
    /// </summary>
    public string? PolicyUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL to the client's terms of service.
    /// </summary>
    public string? TosUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is allowed to remember their consent decision.
    /// </summary>
    public bool AllowRememberConsent { get; set; }

    /// <summary>
    /// Gets or sets the identity scopes requested by the client.
    /// </summary>
    public IEnumerable<ScopeViewModel>? IdentityScopes { get; set; }

    /// <summary>
    /// Gets or sets the API scopes requested by the client.
    /// </summary>
    public IEnumerable<ScopeViewModel>? ApiScopes { get; set; }
}
