// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.Duende.Quickstart.Account;

/// <summary>
/// View model representing the state after a user has logged out.
/// </summary>
public class LoggedOutViewModel
{
    /// <summary>
    /// Gets or sets the URI to redirect to after logout.
    /// </summary>
    public string? PostLogoutRedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the name of the client application.
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// Gets or sets the URL for the sign-out iframe.
    /// </summary>
    public string? SignOutIframeUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically redirect after sign out.
    /// </summary>
    public bool AutomaticRedirectAfterSignOut { get; set; }

    /// <summary>
    /// Gets or sets the logout identifier.
    /// </summary>
    public string? LogoutId { get; set; }

    /// <summary>
    /// Gets a value indicating whether to trigger external sign out.
    /// </summary>
    public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;

    /// <summary>
    /// Gets or sets the external authentication scheme.
    /// </summary>
    public string? ExternalAuthenticationScheme { get; set; }
}