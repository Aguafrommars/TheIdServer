// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
#if DUENDE
using static Duende.IdentityServer.IdentityServerConstants;
#else
using static IdentityServer4.IdentityServerConstants;
#endif

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class ECDsaEncryptorDescriptorTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ECDsaEncryptorDescriptor(null));
            Assert.Throws<ArgumentNullException>(() => new ECDsaEncryptorDescriptor(null, null));
            Assert.Throws<ArgumentNullException>(() => new ECDsaEncryptorDescriptor(new ECDsaEncryptorConfiguration(), null));
        }

        [Fact]
        public async Task ExportToXml_should_export_key_without_ECDsa()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var services = new ServiceCollection();
            var builder = services.AddKeysRotation(RsaSigningAlgorithm.RS256)
                .AddECDsaKeysRotation(ECDsaSigningAlgorithm.ES256)
                .AddECDsaEncryptorConfiguration(ECDsaSigningAlgorithm.ES256, options => { })
                .PersistKeysToFileSystem(new DirectoryInfo(tempDir));

            var provider = services.BuildServiceProvider();
            var keyProvider = provider.GetRequiredService<IKeyRingStore<ECDsaEncryptorConfiguration, ECDsaEncryptor>>();

            var cred = await keyProvider.GetSigningCredentialsAsync().ConfigureAwait(false);

            var sut = new ECDsaEncryptorDescriptor(new ECDsaEncryptorConfiguration(), cred.Key as ECDsaSecurityKey);

            var result = sut.ExportToXml();

            Assert.NotNull(result);
        }
    }
}
