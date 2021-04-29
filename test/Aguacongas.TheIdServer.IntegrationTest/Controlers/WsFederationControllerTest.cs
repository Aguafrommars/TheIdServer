// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.IdentityModel.Protocols.WsFederation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.Controlers
{
    public class WsFederationControllerTest
    {
        [Fact]
        public async Task Metadata_should_return_metadata_document_with_key_rotation()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync("/wsfederation/metadata");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var reader = XmlReader.Create(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            var serializer = new WsFederationMetadataSerializer();
            var metadata = serializer.ReadMetadata(reader);

            Assert.NotNull(metadata);
        }
    }
}
