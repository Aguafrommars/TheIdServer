namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba;

/// <summary>
/// Represents a scope in the CIBA quickstart flow.
/// </summary>
public class ScopeViewModel
{
    /// <summary>
    /// Gets or sets the unique name of the scope.
    /// </summary>
    public string? Name { get; set; }

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
    /// Gets or sets a value indicating whether the scope should be emphasized in the UI.
    /// </summary>
    public bool Emphasize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scope is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scope is checked by default.
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// Gets or sets the resources associated with the scope.
    /// </summary>
    public IEnumerable<ResourceViewModel>? Resources { get; set; }
}