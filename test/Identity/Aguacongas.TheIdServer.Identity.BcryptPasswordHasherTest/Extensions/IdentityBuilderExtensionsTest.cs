using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.BcryptPasswordHasher.Test.Extensions;
public class IdentityBuilderExtensionsTest
{
    [Fact]
    public void AddBcryptPasswordHasher_should_add_bcrypt_password_hasher_services()
    {
        var builder = new ServiceCollection()
            .AddIdentityCore<string>()
            .AddBcryptPasswordHasher<string>();
        var provider = builder.Services.BuildServiceProvider();

        var hasher = provider.GetService<IPasswordHasher<string>>() as BcryptPasswordHasher<string>;

        Assert.NotNull(hasher);
    }
}
