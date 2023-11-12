namespace Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;

/// <summary>
/// Upgrade password hasher options
/// </summary>
public class UpgradePasswordHasherOptions
{
    /// <summary>
    /// Hash prefixes map
    /// </summary>
    public IDictionary<byte, string>? HashPrefixMaps { get; set; }

    /// <summary>
    /// Password hasher to use.
    /// </summary>
    public string? UsePasswordHasherTypeName { get; set; }

    /// <summary>
    /// (optional) after this date hash using old algorithm will be considered invalid to prevent password shucking. It forces the user to update its password.
    /// </summary>
    public DateTime? DeadLineUtc { get; set; }
}
