using Aguacongas.IdentityServer.Admin.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.Controlers
{
    public class RegisterControllerTest
    {
        [Fact]
        public async Task CreateAsync_should_register_a_new_client()
        {
            var sut = TestUtils.CreateTestServer();
            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] { new Claim("role", "Is4-Writer") });

            var client = sut.CreateClient();
            var request = new StringContent(JsonConvert.SerializeObject(new ClientRegisteration
            {
                ClientNames = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Value = "test"
                    }
                },
                GrantTypes = new List<string>
                {
                    "code"
                }
            }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/register", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
