using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Text;

namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;

/// <summary>
/// Argon2 passwor hasher
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly IArgon2Id _argon2Id;
    private readonly IOptions<Argon2PasswordHasherOptions> _options;

    /// <summary>
    /// Initialize a new instance of <see cref="Argon2PasswordHasher{TUser}"/>
    /// </summary>
    /// <param name="argon2Id"><see cref="IArgon2Id"/> implementation</param>
    /// <param name="options">password hasher options</param>
    public Argon2PasswordHasher(IArgon2Id argon2Id, IOptions<Argon2PasswordHasherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(argon2Id);
        ArgumentNullException.ThrowIfNull(options);
        _argon2Id = argon2Id;
        _options = options;
    }

    /// <inheritdoc/>
    public string HashPassword(TUser user, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var passwordSpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(password));
        var hash = _argon2Id.ComputeHash(passwordSpan);
        return Convert.ToBase64String(new byte[]
        {
            _options.Value.HashPrefix
        }
        .Concat(hash.ToArray())
        .ToArray());
    }

    /// <inheritdoc/>
    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        byte[] decodedHashedPassword;
        try
        {
            decodedHashedPassword = Convert.FromBase64String(hashedPassword);
        }
        catch (FormatException)
        {
            return PasswordVerificationResult.Failed;
        }

        var hashSpan = decodedHashedPassword.AsSpan()[1..];

        try
        {
            if (!_argon2Id.VerifyHash(hashSpan,
                new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(providedPassword))))
            {
                return PasswordVerificationResult.Failed;
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            return PasswordVerificationResult.Failed;
        }
        catch(FormatException)
        {
            return PasswordVerificationResult.Failed;
        }

        if (_argon2Id.NeedsRehash(hashSpan)) 
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }

        return PasswordVerificationResult.Success;
    }
}
