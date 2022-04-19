using Aguacongas.TheIdServer.IntegrationTest;
using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Integration.Duende.Test.Areas.Identity.Pages.Manage
{
    [Collection(BlazorAppCollection.Name)]
    public class SessionsTest
    {
        private TheIdServerFactory _factory;
        public SessionsTest(TheIdServerFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_should_read_user_session()
        {
            var testUserService = _factory.Services.GetRequiredService<TestUserService>();
            testUserService.SetTestUser(true, new[] { new Claim("sub", "test") });

            using var client = _factory.CreateClient();
            using var response = await client.GetAsync("Identity/Account/Manage/Sessions");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("No session", content);
        }
    }
}
