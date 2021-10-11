// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Store;
#if DUENDE
using Duende.IdentityServer.Models;
#else
using IdentityServer4.Models;
#endif
using IdentityModel;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var configuration = new Dictionary<string, string>
            {
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] 
                    { 
                        new Claim("role", "Is4-Writer"),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                    });

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
                RedirectUris = new List<string>
                {
                    "http://localhost"
                }
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.NotNull(result.RegistrationToken);
                Assert.NotNull(result.RegistrationUri);
            }

            registration.Jwks = new JsonWebKeys
            {
                Keys = new[]
                {
                    new JsonWebKey
                    {
                        kty = "RSA",
                        e = "AQAB",
                        use = "sig",
                        alg = "RS256",
                        n = "qBulUDaYV027shwCq82LKIevXdQL2pCwXktQgf2TT3c496pxGdRuxcN_MHGKWNOGQsDLuAVk6NjxYF95obDUFrDiugMuXrvptPrTO8dzTX83k_6ngtjOtx2UrTk_7f0EYNrusykrsB-cOvCMREsfktlsavvMKBGrzpxaHlRxcSsMxzB0dddDSlH8mxlzOGcbBuvZnbNg0EUuQC4jvM9Gy6gUEcoU0S19XnUcgwLGLPfIX2dMO4FxTAsaaTYT7msxGMBNIVUTVnL0HctYr0YVYu0hD9rePnvxJ_-OwOdxIETQlR9vp61xFr4juzyyMWTrjCACxxLm-CyEQGjwx2YZaw"
                    }
                }
            };
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
                    .SetTestUser(true, new Claim[] 
                    {
                        new Claim("role", "Is4-Writer"),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                    });

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

            registration.PolicyUris = null;
            registration.GrantTypes = new[] { "invalid" };

            // invalid grant type
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_grant_type", error.Error);
                Assert.Equal("GrantType 'invalid' is not supported.", error.Error_description);
            }

            registration.GrantTypes = null;
            registration.ResponseTypes = new[] { "invalid" };

            // invalid reponse type
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_response_type", error.Error);
                Assert.Equal("ResponseType 'invalid' is not supported.", error.Error_description);
            }

            registration.GrantTypes = new[] { "implicit" };
            registration.RedirectUris = new[]
            {
                "https://test"
            };
            registration.ResponseTypes = new[] { "code" };

            // invalid reponse type
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_response_type", error.Error);
                Assert.Equal("No GrantType 'authorization_code' for ResponseType 'code' found in grant_types.", error.Error_description);
            }

            registration.GrantTypes = null;
            registration.ResponseTypes = new[] { "id_token" };

            // reponse / grant type doesn't match
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_response_type", error.Error);
                Assert.Equal("No GrantType 'implicit' for ResponseType 'id_token' found in grant_types.", error.Error_description);
            }

            registration.ResponseTypes = new[] { "token id_token" };

            // reponse / grant type doesn't match
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_response_type", error.Error);
                Assert.Equal("No GrantType 'implicit' for ResponseType 'token id_token' found in grant_types.", error.Error_description);
            }

            registration.ResponseTypes = new[] { "code token id_token" };
            registration.GrantTypes = new[] { "implicit" };

            // reponse / grant type doesn't match
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_response_type", error.Error);
                Assert.Equal("No GrantType 'authorization_code' for ResponseType 'code token id_token' found in grant_types.", error.Error_description);
            }

            registration.ResponseTypes = new[] { "token" };
            registration.RedirectUris = new[]
            {
                "http://test"
            };
            // invalid scheme for grant type implicit

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("Invalid RedirectUri 'http://test'. Implicit client must use 'https' scheme only.", error.Error_description);
            }

            registration.RedirectUris = new[]
            {
                "https://localhost"
            };
            // invalid host for grant type implicit

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("Invalid RedirectUri 'https://localhost'. Implicit client cannot use 'localhost' host.", error.Error_description);
            }

            registration.RedirectUris = new[]
            {
                "https://localhost"
            };
            // invalid host for grant type implicit

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("Invalid RedirectUri 'https://localhost'. Implicit client cannot use 'localhost' host.", error.Error_description);
            }

            registration.ResponseTypes = null;
            registration.GrantTypes = null;

            registration.ApplicationType = "native";
            registration.RedirectUris = new[]
            {
                "http://test"
            };

            // invalid host for native client

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_redirect_uri", error.Error);
                Assert.Equal("Invalid RedirectUri 'http://test'.Only 'localhost' host is allowed for 'http' scheme and 'native' client.", error.Error_description);
            }

            registration.ApplicationType = "invalid";

            // invalid application type
            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();

                var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

                Assert.Equal("invalid_application_type", error.Error);
                Assert.Equal("ApplicationType 'invalid' is invalid. It must be 'web' or 'native'.", error.Error_description);
            }
        }

        [Theory]
        [InlineData("gopher://test")]
        [InlineData("https://test")]
        [InlineData("news:test@test.com")]
        [InlineData("nntp://test@test.com")]
        public async Task CreateAsync_should_validate_native_redirect_uri_scheme(string redirectUri)
        {
            var sut = TestUtils.CreateTestServer();
            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] 
                    { 
                        new Claim(JwtClaimTypes.Role, "Is4-Writer"),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                    });

            var client = sut.CreateClient();

            var registration = new ClientRegisteration
            {
                RedirectUris = new[]
                {
                    redirectUri
                },
                ApplicationType = "native"
            };

            // not redirect uri
            using var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("/api/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);

            Assert.Equal("invalid_redirect_uri", error.Error);

            var uri = new Uri(redirectUri);

            Assert.Equal($"Invalid RedirectUri '{redirectUri}'.Native client cannot use standard '{uri.Scheme}' scheme, you must use a custom scheme such as 'net.pipe' or 'net.tcp', or 'http' scheme with 'localhost' host.", error.Error_description);
        }

        [Fact]
        public async Task CreateAsync_should_validate_caller()
        {
            var sut = TestUtils.CreateTestServer(configurationOverrides: new Dictionary<string, string>
            {
                ["DynamicClientRegistrationOptions:AllowedContacts:0:Contact"] = "test",
                ["DynamicClientRegistrationOptions:AllowedContacts:0:AllowedHosts:0"] = "localhost",
            });

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
                RedirectUris = new List<string>
                {
                    "http://localhost"
                },
                Contacts = new[]
                {
                    "test"
                }
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.NotNull(result.RegistrationToken);
                Assert.NotNull(result.RegistrationUri);
            }

            registration.RedirectUris = new[]
            {
                "http://forbidenn"
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);


                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }


            registration.Contacts = new[]
            {
                "forbidenn"
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);


                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }


        [Fact]
        public async Task UpdateAsync_should_update_client()
        {
            var sut = TestUtils.CreateTestServer();
            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] 
                    { 
                        new Claim(JwtClaimTypes.Role, "Is4-Writer"),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                    });

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
                RedirectUris = new List<string>
                {
                    "http://localhost"
                }
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                var content = await response.Content.ReadAsStringAsync();
                registration = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.NotNull(registration.RegistrationToken);
            }

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    Content = request,
                    RequestUri = new Uri(registration.RegistrationUri)
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.Null(result.RegistrationToken);
                Assert.Null(result.RegistrationUri);
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
            registration.Contacts = new[]
            {
                "test@test.com"
            };
            registration.ResponseTypes = new[]
            {
                "code"
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    Content = request,
                    RequestUri = new Uri(registration.RegistrationUri)
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.Null(result.RegistrationToken);
                Assert.Null(result.RegistrationUri);
            }

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    Content = request,
                    RequestUri = new Uri(registration.RegistrationUri)
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.Null(result.RegistrationToken);
                Assert.Null(result.RegistrationUri);
            }

            registration.ClientNames = null;
            registration.ClientUris = null;
            registration.LogoUris = null;
            registration.PolicyUris = null;
            registration.TosUris = null;

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    Content = request,
                    RequestUri = new Uri(registration.RegistrationUri)
                };
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.Null(result.RegistrationToken);
                Assert.Null(result.RegistrationUri);
            }
        }

        [Fact]
        public async Task GetAsync_should_return_registration()
        {
            var sut = TestUtils.CreateTestServer();
            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[]
                    { 
                        new Claim(JwtClaimTypes.Role, "Is4-Writer"),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                    });

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
                RedirectUris = new List<string>
                {
                    "https://localhost"
                },
                ClientUris = new List<LocalizableProperty>
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
                },
                LogoUris = new List<LocalizableProperty>
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
                },
                PolicyUris = new List<LocalizableProperty>
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
                },
                TosUris = new List<LocalizableProperty>
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
                },
                ResponseTypes = new[]
                {
                    "code"
                }
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                var content = await response.Content.ReadAsStringAsync();
                registration = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.NotNull(registration.RegistrationToken);
            }

            using (var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(registration.RegistrationUri)
            })
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.Null(result.RegistrationToken);
                Assert.Null(result.RegistrationUri);
            }
        }

        [Fact]
        public async Task DeleteAsync_should_delete_client()
        {
            var sut = TestUtils.CreateTestServer();
            sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] 
                    { 
                        new Claim(JwtClaimTypes.Role, "Is4-Writer"),
                        new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                    });

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
                RedirectUris = new List<string>
                {
                    "http://localhost"
                }
            };

            using (var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync("/api/register", request);

                var content = await response.Content.ReadAsStringAsync();
                registration = JsonConvert.DeserializeObject<ClientRegisteration>(content);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.NotNull(registration.RegistrationToken);
            }

            using (var message = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(registration.RegistrationUri)
            })
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }

            using (var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(registration.RegistrationUri)
            })
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

                using var response = await client.SendAsync(message);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}