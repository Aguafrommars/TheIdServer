using Geralt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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

    [Fact]
    public void VerifyHashedPassword_should_return_fail_invalid_base64_string()
    {
        var settings = new Argon2PasswordHasherOptions();
        var password = Guid.NewGuid().ToString();
        var hash = $"{CreateHash(settings, password)}===";

        var options = Options.Create(settings);
        var sut = new Argon2PasswordHasher<string>(new Argon2Id(options), options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    [SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "Test")]
    public void VerifyHashedPassword_should_return_fail_if_argonid_throw_exception()
    {
        var settings = new Argon2PasswordHasherOptions();
        var password = Guid.NewGuid().ToString();
        var hash = CreateHash(settings, password);

        var options = Options.Create(settings);
        var argon2Id = new ThrowExceptionArgon2Id
        {
            Exception = new ArgumentOutOfRangeException()
        };
        var sut = new Argon2PasswordHasher<string>(argon2Id, options);

        var result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Failed, result);

        argon2Id.Exception = new FormatException();

        result = sut.VerifyHashedPassword(Guid.NewGuid().ToString(), hash, password);

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    private static string CreateHash(Argon2PasswordHasherOptions settings, string password)
    {
        var options = Options.Create(settings);
        var sut = new Argon2PasswordHasher<string>(new Argon2Id(options), options);

        return sut.HashPassword(Guid.NewGuid().ToString(), password);
    }

    class ThrowExceptionArgon2Id : IArgon2Id
    {
        public Exception? Exception { get; set; }
        public Span<byte> ComputeHash(ReadOnlySpan<byte> password)
        {
            throw new NotImplementedException();
        }

        public bool NeedsRehash(ReadOnlySpan<byte> hash)
        {
            throw new NotImplementedException();
        }

        public bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> password)
        {
            throw Exception!;
        }
    }
}