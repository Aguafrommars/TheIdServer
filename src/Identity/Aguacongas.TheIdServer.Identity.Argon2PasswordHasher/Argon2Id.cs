using Geralt;
using Microsoft.Extensions.Options;
using System.Text;

namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;

internal class Argon2Id : IArgon2Id
{
    private const int MaxHashSize = 128;
    private readonly IOptions<Argon2PasswordHasherOptions> _options;

    public Argon2Id(IOptions<Argon2PasswordHasherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }
    public Span<byte> ComputeHash(ReadOnlySpan<byte> password)
    {
        var settings = _options.Value;
        var hash = new Span<char>(new char[MaxHashSize]);
        Argon2id.ComputeHash(hash, password, settings.Interations, settings.Memory);
        var result = new Span<byte>(new byte[hash.Length]);
        Encoding.UTF8.GetBytes(hash, result);
        return result;
    }

    public bool NeedsRehash(ReadOnlySpan<byte> hash)
    {
        var settings = _options.Value;
        var hashSpan = new Span<char>(new char[MaxHashSize]);
        Encoding.UTF8.GetChars(hash, hashSpan);
        return Argon2id.NeedsRehash(hashSpan, settings.Interations, settings.Memory);
    }

    public bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> password)
    {
        var hashSpan = new Span<char>(new char[MaxHashSize]);
        Encoding.UTF8.GetChars(hash, hashSpan);
        return Argon2id.VerifyHash(hashSpan, password);
    }

}
