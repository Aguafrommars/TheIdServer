namespace Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;

/// <summary>
/// bcrypt password hasher options
/// </summary>
public class BcryptPasswordHasherOptions
{
    /// <summary>
    /// Work factor. 11 by default.
    /// </summary>
    public int WorkFactor { get; set; } = 11;

    /// <summary>
    /// Hash prefix: 0xBC by default.
    /// </summary>
    public byte HashPrefix { get; set; } = 0xBC;
}
