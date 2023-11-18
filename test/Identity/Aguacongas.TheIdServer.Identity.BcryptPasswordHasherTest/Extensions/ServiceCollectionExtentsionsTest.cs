using Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.BCryptPasswordHasher.Test.Extensions;
public class ServiceCollectionExtentsionsTest
{
    [Fact]
    public void AddBCryptPasswordHasher_should_add_BCrypt_password_hasher_services()
    {
        var provider = new ServiceCollection().AddBcryptPasswordHasher<string>().BuildServiceProvider();
        
        var hasher = provider.GetService<IPasswordHasher<string>>() as BcryptPasswordHasher<string>;

        Assert.NotNull(hasher);

        // test options validation
        var hash = hasher.HashPassword(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        Assert.NotNull(hash);
    }
}
