// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class RsaEncryptorDescriptorDeserializerTest
    {
        [Fact]
        public void ImportFromXml_should_throw_ArgumentNullException_on_element_null()
        {
            var sut = new RsaEncryptorDescriptorDeserializer();
            Assert.Throws<ArgumentNullException>(() => sut.ImportFromXml(null));
        }
    }
}
