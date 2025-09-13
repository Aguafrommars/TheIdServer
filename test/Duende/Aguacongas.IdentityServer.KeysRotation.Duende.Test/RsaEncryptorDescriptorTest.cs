// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using Duende.IdentityServer;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class RsaEncryptorDescriptorTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new RsaEncryptorDescriptor(null));
            Assert.Throws<ArgumentNullException>(() => new RsaEncryptorDescriptor(null, null));
            Assert.Throws<ArgumentNullException>(() => new RsaEncryptorDescriptor(new RsaEncryptorConfiguration(), null));
        }

        [Fact]
        public async Task ExportToXml_should_export_key_without_Rsa()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var builder = new ServiceCollection()
                .AddKeysRotation(IdentityServerConstants.RsaSigningAlgorithm.RS256)
                .PersistKeysToFileSystem(new DirectoryInfo(tempDir));


            var provider = builder.Services.BuildServiceProvider();
            var keyProvider = provider.GetRequiredService<IKeyRingStore<RsaEncryptorConfiguration, RsaEncryptor>>();

            var cred = await keyProvider.GetSigningCredentialsAsync();

            var sut = new RsaEncryptorDescriptor(new RsaEncryptorConfiguration(), cred.Key as RsaSecurityKey);

            var result = sut.ExportToXml();

            Assert.NotNull(result);
        }
    }
}
