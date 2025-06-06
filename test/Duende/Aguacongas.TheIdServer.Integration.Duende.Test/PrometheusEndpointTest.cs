﻿using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.IntegrationTest;
using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Duende.IntegrationTest
{
    [Collection(BlazorAppCollection.Name)]
    public class PrometheusEndpointTest
    {
        private TheIdServerFactory _factory;
        public PrometheusEndpointTest(TheIdServerFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Metrics_should_return_prometheus_metrics()
        {
            var userService = _factory.Services.GetRequiredService<TestUserService>();
            userService.SetTestUser(true, new[] { new Claim("role", SharedConstants.READERPOLICY) });
            using var client = _factory.CreateClient();
            using var response = await client.GetAsync("/metrics");
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Metrics_should_return_unauthorized_when_protected()
        {
            var userService = _factory.Services.GetRequiredService<TestUserService>();
            userService.SetTestUser(false);
            using var client = _factory.CreateClient();
            using var response = await client.GetAsync("/metrics");
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
