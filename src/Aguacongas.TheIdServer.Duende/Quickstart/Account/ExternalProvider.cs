// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.Duende.Quickstart.Account;

/// <summary>
/// Represents an external authentication provider.
/// </summary>
public class ExternalProvider
{
    /// <summary>
    /// Gets or sets the display name of the external provider.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the authentication scheme used by the external provider.
    /// </summary>
    public string? AuthenticationScheme { get; set; }
}