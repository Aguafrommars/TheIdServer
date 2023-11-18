using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.ScryptPasswordHasher.Test.Extensions;
public class ServiceCollectionExtentsionsTest
{
    [Fact]
    public void AddScryptPasswordHasher_should_add_Scrypt_password_hasher_services()
    {
        var provider = new ServiceCollection().AddScryptPasswordHasher<string>().BuildServiceProvider();
        
        var hasher = provider.GetService<IPasswordHasher<string>>() as ScryptPasswordHasher<string>;

        Assert.NotNull(hasher);

        // test options validation
        var hash = hasher.HashPassword(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        Assert.NotNull(hash);
    }
}
