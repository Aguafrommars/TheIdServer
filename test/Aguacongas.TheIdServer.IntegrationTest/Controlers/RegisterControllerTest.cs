using Aguacongas.IdentityServer.Admin.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

            var registration = new ClientRegisteration
            {
                ClientNames = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Value = "test"
                    },
                },
                GrantTypes = new List<string>
                {
                    "code"
                },
                RedirectUris = new List<string>
                {
                    "http://locahost"
                }
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }

            registration.RedirectUris = new List<string>
            {
                "https://localhost"
            };
            registration.ClientNames = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Culture = "fr-FR",
                        Value = "test"
                    },
                };

            registration.ClientUris = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Value = "https://localhost"
                    },
                    new LocalizableProperty
                    {
                        Culture = "fr-FR",
                        Value = "https://localhost/fr-FR"
                    },
                };

            registration.LogoUris = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Value = "https://localhost"
                    },
                    new LocalizableProperty
                    {
                        Culture = "fr-FR",
                        Value = "https://localhost/fr-FR"
                    },
                };

            registration.PolicyUris = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Value = "https://localhost"
                    },
                    new LocalizableProperty
                    {
                        Culture = "fr-FR",
                        Value = "https://localhost/fr-FR"
                    },
                };

            registration.TosUris = new List<LocalizableProperty>
                {
                    new LocalizableProperty
                    {
                        Value = "https://localhost"
                    },
                    new LocalizableProperty
                    {
                        Culture = "fr-FR",
                        Value = "https://localhost/fr-FR"
                    },
                };

            registration.JwksUri = "https://jwk";

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }


        [Fact]
        public async Task CreateAsync_should_validate_request()
        {
            var sut = TestUtils.CreateTestServer();
            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] { new Claim("role", "Is4-Writer") });

            var client = sut.CreateClient();

            var registration = new ClientRegisteration
            {
            };

            // not redirect uri
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);

                Assert.Equal("RedirectUri is required.", error.Error_description);
            }

            registration.RedirectUris = new List<string>
            {
            };

            // empty redirect uris
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("RedirectUri is required.", error.Error_description);
            }

            registration.RedirectUris = new List<string>
            {
                "test"
            };

            // invalid uri prefix
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("RedirectUri 'test' is not valid.", error.Error_description);
            }


            registration.RedirectUris = new List<string>
            {
                "ssh:test"
            };

            // invalid uri prefix
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("RedirectUri 'ssh:test' uses invalid scheme. If this scheme should be allowed, then configure it via ValidationOptions.", error.Error_description);
            }

            registration.RedirectUris = new List<string>
            {
                "http://localhost"
            };
            registration.LogoUris = new List<LocalizableProperty>
            {
                new LocalizableProperty
                {
                    Value = "test"
                }
            };

            // invalid logo uris
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_logo_uri", error.Error);
                Assert.Equal("LogoUri 'test' is not valid.", error.Error_description);
            }

            registration.LogoUris = new List<LocalizableProperty>
            {
                new LocalizableProperty
                {
                    Value = "http://test"
                }
            };

            // logo uri don't match redirect uri host
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_logo_uri", error.Error);
                Assert.Equal("LogoUri 'http://test' host doesn't match a redirect uri host.", error.Error_description);
            }

            registration.LogoUris = null;
            registration.PolicyUris = new List<LocalizableProperty>
            {
                new LocalizableProperty
                {
                    Value = "test"
                }
            };

            // invalid logo uris
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_policy_uri", error.Error);
                Assert.Equal("PolicyUri 'test' is not valid.", error.Error_description);
            }

            registration.PolicyUris = new List<LocalizableProperty>
            {
                new LocalizableProperty
                {
                    Value = "http://test"
                }
            };

            // policy uri don't match redirect uri host
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_policy_uri", error.Error);
                Assert.Equal("PolicyUri 'http://test' host doesn't match a redirect uri host.", error.Error_description);
            }
        }
    }
}
