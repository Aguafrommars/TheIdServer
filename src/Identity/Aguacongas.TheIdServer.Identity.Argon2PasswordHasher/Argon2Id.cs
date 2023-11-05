using Geralt;
using Microsoft.Extensions.Options;

namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;
internal class Argon2Id : IArgon2Id
{
    private readonly IOptions<Argon2PasswordHasherOptions> _options;

    public Argon2Id(IOptions<Argon2PasswordHasherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }
    public Span<byte> ComputeHash(ReadOnlySpan<byte> password)
    {
        var settings = _options.Value;
        var hash = new Span<byte>(new byte[Argon2id.MaxHashSize]);
        Argon2id.ComputeHash(hash, password, settings.Interations, settings.Memory);
        return hash;
    }

    public bool NeedsRehash(ReadOnlySpan<byte> hash)
    {
        var settings = _options.Value;
        return Argon2id.NeedsRehash(hash, settings.Interations, settings.Memory);
    }

    public bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> password)
    => Argon2id.VerifyHash(hash, password);
    
}
