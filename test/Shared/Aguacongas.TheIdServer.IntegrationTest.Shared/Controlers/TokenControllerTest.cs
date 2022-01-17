using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Store;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.Shared.Controlers
{
    public class TokenControllerTest
    {
        [Fact]
        public async Task CreatePersonalAccessTokenAsync_should_create_PAT()
        {
            using var sut = TestUtils.CreateTestServer();

            var sub = Guid.NewGuid().ToString();
            var role = Guid.NewGuid().ToString();

            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] 
                    {
                        new Claim(JwtClaimTypes.ClientId, "theidserver-swagger"),
                        new Claim(ClaimTypes.NameIdentifier, sub),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.TOKENSCOPES),
                    });

            var client = sut.CreateClient();

            var command = new CreatePersonalAccessToken
            {
                LifetimeDays = 1,
                Apis = new[] { "theidserveradminapi" },
                ClaimTypes = new[] { "name", "role" },
                Scopes = new[] { "theidserveradminapi" }
            };

            using var request = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("api/token/pat", request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var token = JsonSerializer.Deserialize<string>(content);
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtSecurityToken.Audiences, a => a == "theidserveradminapi");
            Assert.Contains(jwtSecurityToken.Claims, c => c.Type == JwtClaimTypes.ClientId && c.Value == "theidserver-swagger");
            Assert.Contains(jwtSecurityToken.Claims, c => c.Type == JwtClaimTypes.Scope && c.Value == "theidserveradminapi");
            Assert.Contains(jwtSecurityToken.Claims, c => c.Type == JwtClaimTypes.Subject && c.Value == sub);
            Assert.Contains(jwtSecurityToken.Claims, c => c.Type == JwtClaimTypes.Name && c.Value == "test");
            Assert.Contains(jwtSecurityToken.Claims, c => c.Type == JwtClaimTypes.Role && c.Value == role);
        }

        [Fact]
        public async Task CreatePersonalAccessTokenAsync_should_return_bad_request_when_client_not_exist()
        {
            using var sut = TestUtils.CreateTestServer();

            var sub = Guid.NewGuid().ToString();
            var role = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();

            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[]
                    {
                        new Claim(JwtClaimTypes.ClientId, clientId),
                        new Claim(ClaimTypes.NameIdentifier, sub),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.TOKENSCOPES),
                    });

            var client = sut.CreateClient();

            var command = new CreatePersonalAccessToken
            {
                LifetimeDays = 1,
                Apis = new[] { "theidserveradminapi" },
                ClaimTypes = new[] { "name", "role" },
                Scopes = new[] { "theidserveradminapi" }
            };

            using var request = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("api/token/pat", request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
            Assert.Equal($"Client not found for client id '{clientId}'.", error.Detail);
        }

        [Fact]
        public async Task CreatePersonalAccessTokenAsync_should_return_bad_request_when_scope_not_allowed()
        {
            using var sut = TestUtils.CreateTestServer();

            var sub = Guid.NewGuid().ToString();
            var role = Guid.NewGuid().ToString();
            var scope = Guid.NewGuid().ToString();

            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[]
                    {
                        new Claim(JwtClaimTypes.ClientId, "theidserver-swagger"),
                        new Claim(ClaimTypes.NameIdentifier, sub),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.TOKENSCOPES),
                    });

            var client = sut.CreateClient();

            var command = new CreatePersonalAccessToken
            {
                LifetimeDays = 1,
                Apis = new[] { "theidserveradminapi" },
                ClaimTypes = new[] { "name", "role" },
                Scopes = new[] { scope }
            };

            using var request = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("api/token/pat", request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
            Assert.Equal($"Scope '{scope}' not found in 'theidserver-swagger' allowed scopes.", error.Detail);
        }

        [Fact]
        public async Task CreatePersonalAccessTokenAsync_should_return_bad_request_when_api_not_exists()
        {
            using var sut = TestUtils.CreateTestServer();

            var sub = Guid.NewGuid().ToString();
            var role = Guid.NewGuid().ToString();
            var api = Guid.NewGuid().ToString();

            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[]
                    {
                        new Claim(JwtClaimTypes.ClientId, "theidserver-swagger"),
                        new Claim(ClaimTypes.NameIdentifier, sub),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.TOKENSCOPES),
                    });

            var client = sut.CreateClient();

            var command = new CreatePersonalAccessToken
            {
                LifetimeDays = 1,
                Apis = new[] { api },
                ClaimTypes = new[] { "name", "role" },
                Scopes = new[] { "theidserveradminapi" }
            };

            using var request = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("api/token/pat", request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
            Assert.Equal($"Api '{api}' not found.", error.Detail);
        }

        [Fact]
        public async Task CreatePersonalAccessTokenAsync_should_return_bad_request_when_apis_is_null()
        {
            using var sut = TestUtils.CreateTestServer();

            var sub = Guid.NewGuid().ToString();
            var role = Guid.NewGuid().ToString();
            var api = Guid.NewGuid().ToString();

            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[]
                    {
                        new Claim(JwtClaimTypes.ClientId, "theidserver-swagger"),
                        new Claim(ClaimTypes.NameIdentifier, sub),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.TOKENSCOPES),
                    });

            var client = sut.CreateClient();

            var command = new CreatePersonalAccessToken
            {
                LifetimeDays = 1,
                ClaimTypes = new[] { "name", "role" },
                Scopes = new[] { "theidserveradminapi" }
            };

            using var request = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("api/token/pat", request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = JsonSerializer.Deserialize<ValidationProblemDetails>(content);
            Assert.Contains(error.Errors, e => e.Key == "Apis" && e.Value.Any(v => v == "The Apis field is required."));
        }
    }
}
