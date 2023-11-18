using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.Argon2PasswordHasher.Test.Extensions;
public class ServiceCollectionExtentsionsTest
{
    [Fact]
    public void AddArgon2PasswordHasher_should_add_argon2_password_hasher_services()
    {
        var provider = new ServiceCollection().AddArgon2PasswordHasher<string>().BuildServiceProvider();
        
        var hasher = provider.GetService<IPasswordHasher<string>>() as Argon2PasswordHasher<string>;

        Assert.NotNull(hasher);

        // test options validation
        var hash = hasher.HashPassword(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        Assert.NotNull(hash);
    }
}
