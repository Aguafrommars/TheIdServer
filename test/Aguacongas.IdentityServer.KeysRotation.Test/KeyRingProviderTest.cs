using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class KeyRingProviderTest
    {
        [Fact]
        public async Task GetCurrentKeyRing_should_create_keys()
        {
            var tempDirectory = Path.GetTempPath();
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToFileSystem(new DirectoryInfo(tempDirectory))
                .ProtectKeysWithCertificate("086e2e047effd5d21d1429245cd28eb14966b257");

            var provider = builder.Services.BuildServiceProvider();
            var validationKeysStore = provider.GetRequiredService<IValidationKeysStore>();

            var keys = await validationKeysStore.GetValidationKeysAsync().ConfigureAwait(false);
            Assert.NotNull(keys);
            Assert.NotEmpty(keys);
            var key = keys.FirstOrDefault();
            Assert.NotNull(key);
            var signingStore = provider.GetRequiredService<ISigningCredentialStore>();
            var cred = await signingStore.GetSigningCredentialsAsync().ConfigureAwait(false);
            Assert.NotNull(cred);
        }
    }
}
