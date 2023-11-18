using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Scrypt;
using System.Text;

namespace Aguacongas.TheIdServer.Identity.ScryptPasswordHasher;

/// <summary>
/// scrypt password hasher
/// </summary>
/// <typeparam name="TUser"></typeparam>

public class ScryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly ScryptEncoder _encoder;
    private readonly IOptions<ScryptPasswordHasherOptions> _options;

    /// <summary>
    /// Initialize a ne instance of <see cref="ScryptPasswordHasher{TUser}"/>
    /// </summary>
    /// <param name="encoder"></param>
    /// <param name="options"></param>
    public ScryptPasswordHasher(ScryptEncoder encoder, IOptions<ScryptPasswordHasherOptions> options)
    {
        _encoder = encoder;
        _options = options;
    }

    /// <inheritdoc/>
    public string HashPassword(TUser user, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var hash = _encoder.Encode(password);
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

        var isValid = _encoder.Compare(providedPassword, hash);

        if (!isValid)
        {
            return PasswordVerificationResult.Failed;
        }

        ScryptPasswordHasher<TUser>.ExtractHeader(hash, out var _, out var iterationCount, out var blockSize, out var threadCount, out var _);
        var settings = _options.Value;
        if (settings.IterationCount != iterationCount || settings.BlockSize != blockSize || settings.ThreadCount != threadCount)
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }
        return PasswordVerificationResult.Success;
    }

    private static void ExtractHeader(string hashedPassword, out int version, out int iterationCount, out int blockSize, out int threadCount, out byte[] saltBytes)
    {
        var parts = hashedPassword.Split('$');

        version = parts[1][1] - '0';

        if (version >= 2)
        {
            iterationCount = Convert.ToInt32(parts[2]);
            blockSize = Convert.ToInt32(parts[3]);
            threadCount = Convert.ToInt32(parts[4]);
            saltBytes = Convert.FromBase64String(parts[5]);
        }
        else
        {
            var config = Convert.ToInt64(parts[2], 16);
            iterationCount = (int)config >> 16 & 0xffff;
            blockSize = (int)config >> 8 & 0xff;
            threadCount = (int)config & 0xff;
            saltBytes = Convert.FromBase64String(parts[3]);
        }
    }

}
