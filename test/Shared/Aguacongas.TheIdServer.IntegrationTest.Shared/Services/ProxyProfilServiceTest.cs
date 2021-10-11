// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Models;
using IdentityModel;
#if DUENDE
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
#else
using IdentityServer4.Models;
using IdentityServer4.Validation;
#endif
using Microsoft.AspNetCore.Builder;
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
            var server = TestUtils.CreateTestServer(configurationOverrides: new Dictionary<string, string>
            {
#if DUENDE
                ["ConnectionStrings:DefaultConnection"] = "Data Source = (LocalDb)\\MSSQLLocalDB; database = TheIdServer.Test.Services.Duende; trusted_connection = yes; "
#else
                ["ConnectionStrings:DefaultConnection"] = "Data Source = (LocalDb)\\MSSQLLocalDB; database = TheIdServer.Test.Services.IS4; trusted_connection = yes; "
#endif
            }, configureEndpoints: (endpoints, isProxy) =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                if (!isProxy)
                {
                    endpoints.MapHub<ProviderHub>("/providerhub");
                }
            });
            var provider = server.Host.Services;

            var testUserService = provider.GetRequiredService<TestUserService>();
            testUserService.SetTestUser(true, new Claim[]
            {
                new Claim(JwtClaimTypes.Role, SharedConstants.READERPOLICY),
                new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
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
                RequestedResources = new ResourceValidationResult
                {
                    Resources = new Resources
                    {
                        IdentityResources = new List<IdentityResource>
                        {
                            new IdentityResource
                            {
                                Name= "test",
                                Properties = new Dictionary<string, string>
                                {
                                    [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ProxyProfilServiceTest).FullName
                                }
                            }
                        }
                    }
                }
            };

            await sut.GetProfileDataAsync(context).ConfigureAwait(false);

            Assert.Empty(context.IssuedClaims);
        }
    }
}
