using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.TheIdServer.Identity.UpgradePasswordHasher.Test.Extensions;
public class IdentityBuilderExtensionsTest
{
    [Fact]
    public void AddUpgradePasswordHasher_should_add_upgrade_password_hasher_services()
    {
        var builder = new ServiceCollection()
            .AddIdentityCore<string>()
            .AddUpgradePasswordHasher<string>();
        var provider = builder.Services.BuildServiceProvider();

        var hasher = provider.GetService<IPasswordHasher<string>>() as UpgradePasswordHasher<string>;

        Assert.NotNull(hasher);
    }
}
