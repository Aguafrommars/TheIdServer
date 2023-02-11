// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class ECDsaEncryptorTest
    {
        [Fact]
        public void Decrypt_should_throw_NotImplementedException()
        {
            var sut = new ECDsaEncryptor(null);
            Assert.Throws<NotImplementedException>(() => sut.Decrypt(null, null));
        }

        [Fact]
        public void Encrypt_should_throw_NotImplementedException()
        {
            var sut = new ECDsaEncryptor(null);
            Assert.Throws<NotImplementedException>(() => sut.Encrypt(null, null));
        }
    }
}
