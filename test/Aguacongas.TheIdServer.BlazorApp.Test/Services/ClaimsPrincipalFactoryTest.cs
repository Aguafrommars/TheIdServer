// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Services
{
    public class ClaimsPrincipalFactoryTest
    {
        [Fact]
        public async Task CreateUserAsync_should_transform_array_claims()
        {
            var accessorMock = new Mock<IAccessTokenProviderAccessor>();
            var loggerMock = new Mock<ILogger<ClaimsPrincipalFactory>>();

            var sut = new ClaimsPrincipalFactory(accessorMock.Object, loggerMock.Object);

            var user = await sut.CreateUserAsync(new RemoteUserAccount
            {
                AdditionalProperties = new Dictionary<string, object>
                {
                    ["name"] = "name",
                    ["role"] = "[\"role\"]"
                }
            }, new RemoteAuthenticationUserOptions
            {
                AuthenticationType = "oidc",
                NameClaim = "name",
                RoleClaim = "role"
            });

            Assert.Contains(user.Claims, c => c.Type == "role" && c.Value == "role");
        }
    }
}
