using Aguacongas.IdentityServer.Admin.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Configuration
{
    public class ConfigureSigningCredentialsTest
    {
        [Fact]
        public void Configure_should_throw_when_credential_file_not_exists()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Type"] = "File",
                ["FilePath"] = "cred.pfx"
            })
                .Build();
            var loggerMock = new Mock<ILogger<ConfigureSigningCredentials>>();

            var sut = new ConfigureSigningCredentials(configuration, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Configure(new Options.CredentialsOptions()));
        }

        [Fact]
        public void Configure_should_throw_when_no_password()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Type"] = "File",
                ["FilePath"] = "invalidcred.pfx"
            })
                .Build();
            var loggerMock = new Mock<ILogger<ConfigureSigningCredentials>>();

            var sut = new ConfigureSigningCredentials(configuration, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Configure(new Options.CredentialsOptions()));
        }

        [Fact]
        public void Configure_should_throw_when_credentials_file_invalid()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Type"] = "File",
                ["FilePath"] = "invalidcred.pfx",
                ["StorageFlags"] = "",
                ["Password"] = "test"
            })
                .Build();
            var loggerMock = new Mock<ILogger<ConfigureSigningCredentials>>();

            var sut = new ConfigureSigningCredentials(configuration, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Configure(new Options.CredentialsOptions()));
        }

        [Fact]
        public void Configure_should_throw_when_invalid_storage_flag()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Type"] = "File",
                ["FilePath"] = "invalidcred.pfx",
                ["StorageFlags"] = "test"
            })
                .Build();
            var loggerMock = new Mock<ILogger<ConfigureSigningCredentials>>();

            var sut = new ConfigureSigningCredentials(configuration, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Configure(new Options.CredentialsOptions()));
        }


        [Fact]
        public void Configure_should_throw_when_store_location_invalid()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Type"] = "Store",
            })
                .Build();
            var loggerMock = new Mock<ILogger<ConfigureSigningCredentials>>();

            var sut = new ConfigureSigningCredentials(configuration, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Configure(new Options.CredentialsOptions()));
        }

        [Fact]
        public void Configure_should_throw_when_cred_not_found()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Type"] = "Store",
                ["StoreLocation"] = "LocalMachine",
                ["StoreName"] = "notfound",
                ["Name"] = "notfound"
            })
                .Build();
            var loggerMock = new Mock<ILogger<ConfigureSigningCredentials>>();

            var sut = new ConfigureSigningCredentials(configuration, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Configure(new Options.CredentialsOptions()));
        }
    }
}
