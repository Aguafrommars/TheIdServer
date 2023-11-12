namespace Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;

/// <summary>
/// Upgrade password hasher options
/// </summary>
public class UpgradePasswordHasherOptions
{
    /// <summary>
    /// Hash prefixes map
    /// </summary>
    public IDictionary<byte, string> HashPrefixMaps { get; set; } = new Dictionary<byte, string>
    {
        [0x00] = "Microsoft.AspNetCore.Identity.PasswordHasher",
        [0x01] = "Microsoft.AspNetCore.Identity.PasswordHasher",
        [0xA2] = "Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Argon2PasswordHasher",
        [0x0C] = "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher",
        [0xBC] = "Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.BcryptPasswordHasher"
    };

    /// <summary>
    /// Password hasher to use.
    /// </summary>
    public string UsePasswordHasherTypeName { get; set; } = "Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Argon2PasswordHasher";

    /// <summary>
    /// (optional) after this date hash using old algorithm will be considered invalid to prevent password shucking. It forces the user to update its password.
    /// </summary>
    public DateTime? DeadLineUtc { get; set; }
}
