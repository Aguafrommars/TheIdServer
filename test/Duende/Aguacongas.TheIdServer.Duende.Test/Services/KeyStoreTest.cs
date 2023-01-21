// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.TheIdServer.Services;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Test.Services
{
    public class KeyStoreTest
    {
        [Fact]
        public void Constructor_should_check_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new KeyStore<string, IAuthenticatedEncryptorDescriptor>(null));
        }

        [Fact]
        public async Task GetAllKeysAsync_should_return_page_method()
        {
            var keyManagerMock = new Mock<IKeyManager>();
            keyManagerMock.Setup(m => m.GetAllKeys()).Returns(new List<IKey>()).Verifiable();
            var defaultResolverMock = new Mock<IDefaultKeyResolver>();
            var providerClientMock = new Mock<IProviderClient>();
            var wrapper = new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new[] { new Tuple<IKeyManager, string, IEnumerable<IKey>>(keyManagerMock.Object, "test", new List<IKey>()) }, defaultResolverMock.Object, providerClientMock.Object);

            var sut = new KeyStore<string, IAuthenticatedEncryptorDescriptor>(wrapper);

            var page = await sut.GetAllKeysAsync().ConfigureAwait(false);

            Assert.Empty(page.Items);
        }

        [Fact]
        public async Task RevokeKeyAsync_should_not_be_implemented()
        {
            var keyManagerMock = new Mock<IKeyManager>();
            keyManagerMock.Setup(m => m.GetAllKeys()).Returns(new List<IKey>()).Verifiable();
            var defaultResolverMock = new Mock<IDefaultKeyResolver>();
            var providerClientMock = new Mock<IProviderClient>();
            var wrapper = new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new[] { new Tuple<IKeyManager, string, IEnumerable<IKey>>(keyManagerMock.Object, "test", new List<IKey>()) }, defaultResolverMock.Object, providerClientMock.Object);

            var sut = new KeyStore<string, IAuthenticatedEncryptorDescriptor>(wrapper);

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.RevokeKeyAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).ConfigureAwait(false);
        }
    }
}
