using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Test;

public class Argon2PasswordHasherTest
{
    [Fact]
    public void HashPassword_should_hash_password_according_to_options()
    {
        var settings = new Argon2PasswordHasherOptions();
        
        var result = CreateHash(settings, Guid.NewGuid().ToString());

        Assert.NotNull(result);
        var binaryResult = Convert.FromBase64String(result);

        Assert.Equal(settings.HashPrefix, binaryResult[0]);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_success_on_password_valid()
    {
        var settings = new Argon2PasswordHasherOptions();
        var options = Options.Create(settings);
        var sut = new Argon2PasswordHasher<string>(new Argon2Id(options), options);
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_rehash_needed_on_settings_change()
    {
        var settings = new Argon2PasswordHasherOptions
        {
            Interations = 3
        };
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        settings.Interations = 2;
        var options = Options.Create(settings);
        var sut = new Argon2PasswordHasher<string>(new Argon2Id(options), options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_fail_invalid_hash()
    {
        var settings = new Argon2PasswordHasherOptions();
        var hash = CreateHash(settings, Guid.NewGuid().ToString());

        var options = Options.Create(settings);
        var sut = new Argon2PasswordHasher<string>(new Argon2Id(options), options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, Guid.NewGuid().ToString());

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    private static string CreateHash(Argon2PasswordHasherOptions settings, string password)
    {
        var options = Options.Create(settings);
        var sut = new Argon2PasswordHasher<string>(new Argon2Id(options), options);

        return sut.HashPassword(Guid.NewGuid().ToString(), password);
    }
}