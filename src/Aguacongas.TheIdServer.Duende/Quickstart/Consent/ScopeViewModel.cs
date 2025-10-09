// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.Duende.Quickstart.Consent;

/// <summary>
/// Represents a scope in the consent screen.
/// </summary>
public class ScopeViewModel
{
    /// <summary>
    /// Gets or sets the value of the scope.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the display name of the scope.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the description of the scope.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scope should be emphasized.
    /// </summary>
    public bool Emphasize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scope is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scope is checked.
    /// </summary>
    public bool Checked { get; set; }
}
