// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Validators;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Validators
{
    public class UniqueValidatorTest
    {
        [Fact]
        public void IsValid_should_check_doublon()
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ProtocolType = "oidc",
                AllowedGrantTypes = new List<ClientGrantType>
                {
                    new ClientGrantType
                    {
                        GrantType = "custom"
                    }
                },
                AllowedScopes = new List<ClientScope>(),
                RedirectUris = new List<ClientUri>
                {
                    new ClientUri
                    {
                        Uri = "https://exemple.com"
                    },
                    new ClientUri
                    {
                        Uri = "https://exemple.com"
                    }
                },
                Properties = new List<ClientProperty>(),
                Resources = new List<ClientLocalizedResource>()
            };

            var localizerMock = new Mock<IStringLocalizer>();
            localizerMock.SetupGet(m => m[It.IsAny<string>()]).Returns(new LocalizedString(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            var validator = new ClientValidator(client, localizerMock.Object);

            var result = validator.Validate(client);

            Assert.Equal(2, result.Errors.Count);

            client.RedirectUris.Remove(client.RedirectUris.First());

            result = validator.Validate(client);

            Assert.Empty(result.Errors);
        }
    }
}
