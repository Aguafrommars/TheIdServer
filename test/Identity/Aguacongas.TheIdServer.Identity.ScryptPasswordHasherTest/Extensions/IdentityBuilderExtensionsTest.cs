using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.Test.Extensions;
public class IdentityBuilderExtensionsTest
{
    [Fact]
    public void AddScryptPasswordHasher_should_add_scrypt_password_hasher_services()
    {
        var builder = new ServiceCollection()
            .AddIdentityCore<string>()
            .AddScryptPasswordHasher<string>();
        var provider = builder.Services.BuildServiceProvider();

        var hasher = provider.GetService<IPasswordHasher<string>>() as ScryptPasswordHasher<string>;

        Assert.NotNull(hasher);
    }
}
