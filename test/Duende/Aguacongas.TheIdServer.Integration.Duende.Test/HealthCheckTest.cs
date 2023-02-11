using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Duende.IntegrationTest
{
    [Collection(BlazorAppCollection.Name)]
    public class HealthCheckTest
    {
        private TheIdServerFactory _factory;
        public HealthCheckTest(TheIdServerFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Healthz_should_return_health_status()
        {
            using var client = _factory.CreateClient();
            using var response = await client.GetAsync("/healthz");

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Healthy", content);
        }
    }
}
