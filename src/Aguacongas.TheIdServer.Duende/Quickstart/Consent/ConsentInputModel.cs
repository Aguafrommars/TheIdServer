// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.Duende.Quickstart.Consent;
/// <summary>
/// Represents the input model for user consent in the authentication flow.
/// </summary>
public class ConsentInputModel
{
    /// <summary>
    /// Gets or sets the button pressed by the user (e.g., "yes", "no").
    /// </summary>
    public string? Button { get; set; }

    /// <summary>
    /// Gets or sets the scopes that the user has consented to.
    /// </summary>
    public IEnumerable<string>? ScopesConsented { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user's consent should be remembered.
    /// </summary>
    public bool RememberConsent { get; set; }

    /// <summary>
    /// Gets or sets the return URL after consent is processed.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets an optional description provided by the user.
    /// </summary>
    public string? Description { get; set; }
}