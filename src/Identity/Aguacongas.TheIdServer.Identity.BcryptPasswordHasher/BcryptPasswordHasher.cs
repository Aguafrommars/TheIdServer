using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;

/// <summary>
/// bcrypt password hasher
/// </summary>
/// <typeparam name="TUser"></typeparam>

public class BcryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly IOptions<BcryptPasswordHasherOptions> _options;

    /// <summary>
    /// Initialize a new instance of <see cref="BcryptPasswordHasher{TUser}"/>
    /// </summary>
    /// <param name="options"></param>
    public BcryptPasswordHasher(IOptions<BcryptPasswordHasherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    /// <inheritdoc/>
    public string HashPassword(TUser user, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var hash = BC.HashPassword(password, _options.Value.WorkFactor);

        return Convert.ToBase64String(new byte[]
        {
            _options.Value.HashPrefix
        }
        .Concat(Encoding.UTF8.GetBytes(hash))
        .ToArray());
    }

    /// <inheritdoc/>
    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        var decodedHashedPassword = Convert.FromBase64String(hashedPassword);
        var hash = Encoding.UTF8.GetString(decodedHashedPassword[1..]);

        if (!BC.Verify(providedPassword, hash))
        {
            return PasswordVerificationResult.Failed;
        }

        if (BC.PasswordNeedsRehash(hash, _options.Value.WorkFactor))
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }

        return PasswordVerificationResult.Success;
    }
}
