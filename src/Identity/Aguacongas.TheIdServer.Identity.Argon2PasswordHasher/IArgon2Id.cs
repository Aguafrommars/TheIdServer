namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;

/// <summary>
/// Implement Argon2
/// </summary>
public interface IArgon2Id
{
    /// <summary>
    /// Compute the hash
    /// </summary>
    /// <param name="password"></param>
    Span<byte> ComputeHash(ReadOnlySpan<byte> password);
    
    /// <summary>
    /// Verify the hash
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> password);
    
    /// <summary>
    /// Verify if the hash needs to be rehashed
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    bool NeedsRehash(ReadOnlySpan<byte> hash);
}
