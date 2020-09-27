using Aguacongas.IdentityServer.Admin.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class KeyRingProviderTest
    {
        [Fact]
        public async Task GetCurrentKeyRing_should_create_keys_and_cache()
        {
            var certificate = SigningKeysLoader.LoadFromFile("theidserver.pfx", "YourSecurePassword", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);
            var tempDirectory = Path.GetTempPath();
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToFileSystem(new DirectoryInfo(tempDirectory))
                .ProtectKeysWithCertificate(certificate);

            var provider = builder.Services.BuildServiceProvider();
            var sut = provider.GetRequiredService<IKeyRingStores>();

            var cred = await sut.GetSigningCredentialsAsync().ConfigureAwait(false);
            Assert.NotNull(cred);

            sut = provider.GetRequiredService<IKeyRingStores>();
            var newCred = await sut.GetSigningCredentialsAsync().ConfigureAwait(false);

            Assert.Equal(cred.Key.KeyId, newCred.Key.KeyId);
        }

        [Fact]
        public async Task GetCurrentKeyRing_should_not_return_revoked_keys()
        {
            var certificate = SigningKeysLoader.LoadFromFile("theidserver.pfx", "YourSecurePassword", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);

            var tempDirectory = Path.GetTempPath();
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToFileSystem(new DirectoryInfo(tempDirectory))
                .ProtectKeysWithCertificate(certificate);

            var provider = builder.Services.BuildServiceProvider();
            var sut = provider.GetRequiredService<IKeyRingStores>();

            var keys = await sut.GetValidationKeysAsync().ConfigureAwait(false);
            Assert.NotNull(keys);
            Assert.NotEmpty(keys);

            var defaultKeyId = sut.DefaultKeyId;
            var cacheableKeyRingProvider = provider.GetRequiredService<ICacheableKeyRingProvider>();
            cacheableKeyRingProvider.KeyManager.RevokeKey(defaultKeyId, "test");
            sut = provider.GetRequiredService<IKeyRingStores>();
            var newKeys = await sut.GetValidationKeysAsync().ConfigureAwait(false);

            Assert.DoesNotContain(keys, k => newKeys.Any(nk => nk.Key.KeyId == k.Key.KeyId));
        }
    }
}
