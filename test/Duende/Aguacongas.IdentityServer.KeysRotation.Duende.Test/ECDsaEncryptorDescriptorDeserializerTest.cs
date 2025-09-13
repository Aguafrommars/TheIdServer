// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class ECDsaEncryptorDescriptorDeserializerTest
    {
        [Fact]
        public void ImportFromXml_should_throw_ArgumentNullException_on_element_null()
        {
            var sut = new ECDsaEncryptorDescriptorDeserializer();
            Assert.Throws<ArgumentNullException>(() => sut.ImportFromXml(null));
        }
    }
}
