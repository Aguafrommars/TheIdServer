using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.Test;

public class BcryptPasswordHasherTest
{
    [Fact]
    public void HashPassword_should_hash_password_according_to_options()
    {
        var settings = new BcryptPasswordHasherOptions();

        var result = CreateHash(settings, Guid.NewGuid().ToString());

        Assert.NotNull(result);
        var binaryResult = Convert.FromBase64String(result);

        Assert.Equal(settings.HashPrefix, binaryResult[0]);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_success_on_password_valid()
    {
        var settings = new BcryptPasswordHasherOptions();
        var options = Options.Create(settings);
        var sut = new BcryptPasswordHasher<string>(options);
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_rehash_needed_on_settings_change()
    {
        var settings = new BcryptPasswordHasherOptions
        {
            WorkFactor = 11
        };
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        settings.WorkFactor = 12;
        var options = Options.Create(settings);
        var sut = new BcryptPasswordHasher<string>(options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_fail_invalid_hash()
    {
        var settings = new BcryptPasswordHasherOptions();
        var hash = CreateHash(settings, Guid.NewGuid().ToString());

        var options = Options.Create(settings);
        var sut = new BcryptPasswordHasher<string>(options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, Guid.NewGuid().ToString());

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_fail_invalid_base64_string()
    {
        var settings = new BcryptPasswordHasherOptions();
        var password = Guid.NewGuid().ToString();
        var hash = $"{CreateHash(settings, password)}===";

        var options = Options.Create(settings);
        var sut = new BcryptPasswordHasher<string>(options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    private static string CreateHash(BcryptPasswordHasherOptions settings, string password)
    {
        var options = Options.Create(settings);
        var sut = new BcryptPasswordHasher<string>(options);

        return sut.HashPassword(Guid.NewGuid().ToString(), password);
    }
}