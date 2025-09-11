// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Services.Test
{
    public class KeyManagerWrapperTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new[] { new Tuple<IKeyManager, string, IEnumerable<IKey>>(new Mock<IKeyManager>().Object, "test", Array.Empty<IKey>()) }, null, null));
            Assert.Throws<ArgumentNullException>(() => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new[] { new Tuple<IKeyManager, string, IEnumerable<IKey>>(new Mock<IKeyManager>().Object, "test", Array.Empty<IKey>()) }, new Mock<IDefaultKeyResolver>().Object, null));
        }
    }
}
