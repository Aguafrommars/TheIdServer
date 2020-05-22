using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.TheIdServer.Models;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.Services
{

    public class ProxyProfilServiceTest
    {

        [Fact]
        public async Task GetProfileDataAsync_should_forward_request_to_webservice()
        {
            var server = TestUtils.CreateTestServer();
            var provider = server.Host.Services;

            var testUserService = provider.GetRequiredService<TestUserService>();
            testUserService.SetTestUser(true, new Claim[]
            {
                new Claim("role", "Is4-Reader")
            });

            var manager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await manager.FindByNameAsync("alice").ConfigureAwait(false);

            var httpClient = server.CreateClient();
            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "/api");

            var sut = new ProxyProfilService<ApplicationUser>(httpClient,
                manager,
                provider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                provider.GetRequiredService<IEnumerable<IProvideClaims>>(),
                provider.GetRequiredService<ILogger<ProxyProfilService<ApplicationUser>>>());

            var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtClaimTypes.Subject, user.Id) })),
                new Client(), "test",
                new string[] { "test" })
            {
                RequestedResources = new Resources
                {
                    IdentityResources = new List<IdentityResource>
                    {
                        new IdentityResource
                        {
                            Name= "profile"
                        }
                    },
                }
            };

            await sut.GetProfileDataAsync(context).ConfigureAwait(false);

            Assert.Empty(context.IssuedClaims);
        }
    }
}
