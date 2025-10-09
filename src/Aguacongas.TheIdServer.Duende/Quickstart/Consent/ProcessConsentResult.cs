// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Models;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Consent;

/// <summary>
/// Represents the result of processing a user consent request in the authentication flow.
/// </summary>
public class ProcessConsentResult
{
    /// <summary>
    /// Gets a value indicating whether the result is a redirect.
    /// </summary>
    public bool IsRedirect => RedirectUri != null;

    /// <summary>
    /// Gets or sets the URI to redirect to after consent processing.
    /// </summary>
    public string? RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the client associated with the consent request.
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Gets a value indicating whether the consent view should be shown.
    /// </summary>
    public bool ShowView => ViewModel != null;

    /// <summary>
    /// Gets or sets the view model for the consent screen.
    /// </summary>
    public ConsentViewModel? ViewModel { get; set; }

    /// <summary>
    /// Gets a value indicating whether there is a validation error.
    /// </summary>
    public bool HasValidationError => ValidationError != null;

    /// <summary>
    /// Gets or sets the validation error message, if any.
    /// </summary>
    public string? ValidationError { get; set; }
}
