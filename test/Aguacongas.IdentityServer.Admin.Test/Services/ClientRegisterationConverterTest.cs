// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class ClientRegisterationConverterTest
    {
        [Fact]
        public void ReadJson_should_deserialize_localizable_properties()
        {
            var serialized = "{ \"client_name#fr-FR\": \"test\", \"redirect_uris\": [\"test\"], \"application_type\": \"native\" }";

            var result = JsonConvert.DeserializeObject<ClientRegisteration>(serialized);

            Assert.Contains(result.ClientNames, n => n.Culture == "fr-FR" && n.Value == "test");
        }

        [Fact]
        public void WriteJson_should_serialize_localizable_properties()
        {
            var registration = new ClientRegisteration
            {
                ClientNames = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Culture = "fr-FR",
                        Value = "test"
                    }
                },
                ApplicationType = "native",
                Contacts = new List<string>
                {
                    "test"
                }
            };

            var result = JsonConvert.SerializeObject(registration);

            Assert.Contains("client_name#fr-FR", result);
        }
    }
}
