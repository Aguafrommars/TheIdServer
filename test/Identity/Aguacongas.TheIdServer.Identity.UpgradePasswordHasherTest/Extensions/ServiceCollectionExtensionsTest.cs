using Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.UpgradePasswordHasherTest.Extensions;
public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddUpgradePasswordHasher_should_add_upgrade_password_hasher_services()
    {
        var services = new ServiceCollection();
        services.AddIdentityCore<string>();

        var provider = services.AddArgon2PasswordHasher<string>()
            .AddBcryptPasswordHasher<string>()
            .AddScryptPasswordHasher<string>()
            .AddUpgradePasswordHasher<string>()
            .BuildServiceProvider();

        using var scope = provider.CreateScope();
        var hasher = scope.ServiceProvider.GetService<IPasswordHasher<string>>() as UpgradePasswordHasher<string>;

        Assert.NotNull(hasher);

        // test options validation
        var hash = hasher.HashPassword(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        Assert.NotNull(hash);
    }

}
