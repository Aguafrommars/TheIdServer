using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Test.Extensions;
public class IdentityBuilderExtensionsTest
{
    [Fact]
    public void AddArgon2PasswordHasher_should_add_argon2_password_hasher_services()
    {
        var builder = new ServiceCollection()
            .AddIdentityCore<string>()
            .AddArgon2PasswordHasher<string>();
        var provider = builder.Services.BuildServiceProvider();

        var hasher = provider.GetService<IPasswordHasher<string>>() as Argon2PasswordHasher<string>;

        Assert.NotNull(hasher);
    }
}
