using Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aguacongas.TheIdServer.Identity.UpgradePasswordHasherTest;

public class UpgradePasswordHasherTest
{
    [Fact]
    public void HashPassword_should_hash_password_with_configured_hascher()
    {
        var settings = new UpgradePasswordHasherOptions
        {
            HashPrefixMaps = new Dictionary<byte, string>
            {
                [0x0C] = "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher"
            },
            UsePasswordHasherTypeName = "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher"
        };
        var provider = new ServiceCollection().AddScryptPasswordHasher<string>()
            .BuildServiceProvider();

        var sut = new UpgradePasswordHasher<string>(provider, Options.Create(settings!));

        var hash = sut.HashPassword(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        Assert.NotNull(hash);

        var decodedHash = Convert.FromBase64String(hash);
        Assert.Equal(0x0C, decodedHash[0]);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_success_on_password_valid()
    {
        var settings = new UpgradePasswordHasherOptions
        {
            HashPrefixMaps = new Dictionary<byte, string>
            {
                [0x0C] = "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher"
            },
            UsePasswordHasherTypeName = "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher"
        };
        var provider = new ServiceCollection().AddScryptPasswordHasher<string>()
            .BuildServiceProvider();

        var sut = new UpgradePasswordHasher<string>(provider, Options.Create(settings!));

        var user = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var hash = sut.HashPassword(user, password);
        var result = sut.VerifyHashedPassword(user, hash, password);

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_rehash_needed_on_password_valid_and_configuration_changed()
    {
        var settings = new UpgradePasswordHasherOptions
        {
            HashPrefixMaps = new Dictionary<byte, string>
            {
                [0x0C] = "Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.ScryptPasswordHasher",
                [0xBC] = "Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.BcryptPasswordHasher"
            },
            UsePasswordHasherTypeName = "Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.BcryptPasswordHasher"
        };
        var provider = new ServiceCollection()
            .AddBcryptPasswordHasher<string>()
            .AddScryptPasswordHasher<string>()
            .BuildServiceProvider();

        var scryptHasher = provider.GetRequiredService<IPasswordHasher<string>>();
        var user = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var hash = scryptHasher.HashPassword(user, password);

        var sut = new UpgradePasswordHasher<string>(provider, Options.Create(settings!));

        var result = sut.VerifyHashedPassword(user, hash, password);

        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_failed_on_dead_line_expired_and_rehash_needed()
    {
        var settings = new UpgradePasswordHasherOptions
        {
            HashPrefixMaps = new Dictionary<byte, string>
            {
                [0xA2] = "Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Argon2PasswordHasher",
                [0xBC] = "Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.BcryptPasswordHasher"
            },
            UsePasswordHasherTypeName = "Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Argon2PasswordHasher",
            DeadLineUtc = DateTime.UtcNow.AddDays(-1)
        };
        var provider = new ServiceCollection()
            .AddArgon2PasswordHasher<string>()
            .AddBcryptPasswordHasher<string>()
            .BuildServiceProvider();

        var bcryptHasher = provider.GetRequiredService<IPasswordHasher<string>>();
        var user = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var hash = bcryptHasher.HashPassword(user, password);

        var sut = new UpgradePasswordHasher<string>(provider, Options.Create(settings!));

        var result = sut.VerifyHashedPassword(user, hash, password);

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }
}