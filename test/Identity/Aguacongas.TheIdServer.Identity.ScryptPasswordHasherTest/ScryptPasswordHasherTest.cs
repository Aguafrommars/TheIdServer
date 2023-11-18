using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Scrypt;

namespace Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.Test;

public class ScryptPasswordHasherTest
{
    [Fact]
    public void HashPassword_should_hash_password_according_to_options()
    {
        var settings = new ScryptPasswordHasherOptions();
        
        var result = CreateHash(settings, Guid.NewGuid().ToString());

        Assert.NotNull(result);
        var binaryResult = Convert.FromBase64String(result);

        Assert.Equal(settings.HashPrefix, binaryResult[0]);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_success_on_password_valid()
    {
        var settings = new ScryptPasswordHasherOptions();
        var options = Options.Create(settings);
        var sut = new ScryptPasswordHasher<string>(new ScryptEncoder(settings.IterationCount, settings.BlockSize, settings.ThreadCount), 
            options);
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_rehash_needed_on_settings_change()
    {
        var settings = new ScryptPasswordHasherOptions
        {
            IterationCount = 4
        };
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        settings.IterationCount = 2;
        var options = Options.Create(settings);
        var sut = new ScryptPasswordHasher<string>(new ScryptEncoder(settings.IterationCount, settings.BlockSize, settings.ThreadCount), options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
    }

    [Fact]
    public void VerifyHashedPassword_should_return_fail_invalid_hash()
    {
        var settings = new ScryptPasswordHasherOptions();
        var hash = CreateHash(settings, Guid.NewGuid().ToString());

        var options = Options.Create(settings);
        var sut = new ScryptPasswordHasher<string>(new ScryptEncoder(settings.IterationCount, settings.BlockSize, settings.ThreadCount), options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, Guid.NewGuid().ToString());

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    private static string CreateHash(ScryptPasswordHasherOptions settings, string password)
    {
        var options = Options.Create(settings);
        var sut = new ScryptPasswordHasher<string>(new ScryptEncoder(settings.IterationCount, settings.BlockSize, settings.ThreadCount), options);

        return sut.HashPassword(Guid.NewGuid().ToString(), password);
    }
}