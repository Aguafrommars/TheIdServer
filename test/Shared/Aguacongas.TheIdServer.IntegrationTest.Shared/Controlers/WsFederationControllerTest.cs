// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using IdentityModel;
#if DUENDE
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using ISModels = Duende.IdentityServer.Models;
#else
using IdentityServer4;
using IdentityServer4.Services;
using ISModels = IdentityServer4.Models;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.Controlers
{
    public class WsFederationControllerTest
    {
        [Fact]
        public async Task Metadata_should_return_metadata_document_with_key_rotation()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync("/wsfederation/metadata");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var reader = XmlReader.Create(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            var serializer = new WsFederationMetadataSerializer();
            var metadata = serializer.ReadMetadata(reader);

            Assert.NotNull(metadata);
        }

        [Fact]
        public async Task Index_should_return_bad_request_when_request_is_bad()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync("/wsfederation");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Index_should_return_bad_request_when_realm_is_not_found()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync("/wsfederation?wtrealm=notfound&wa=wsignin1.0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Index_should_return_bad_request_when_realm_is_not_wsfed_client()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new IdentityServer.Store.Entity.Client
            {
                Id = clientId,
                Enabled = true
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Index_should_return_bad_request_when_relyparty_is_not_found()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new IdentityServer.Store.Entity.Client
            {
                Id = clientId,
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Index_should_wreply_nor_1st_redirect_uri_are_valid_uri()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new Client
            {
                Id = clientId,
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation,
                RelyingParty = new RelyingParty
                {
                    Id = clientId,
                    TokenType = IdentityServer4.WsFederation.WsFederationConstants.TokenTypes.Saml11TokenProfile11,
                    DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                    SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                    SamlNameIdentifierFormat = IdentityServer4.WsFederation.WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString
                }
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Index_should_redirect_to_login_page_when_user_not_found_in_session()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "KeysRotation",
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new Client
            {
                Id = clientId,
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation,
                RelyingParty = new RelyingParty
                {
                    Id = clientId,
                    TokenType = IdentityServer4.WsFederation.WsFederationConstants.TokenTypes.Saml11TokenProfile11,
                    DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                    SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                    SamlNameIdentifierFormat = IdentityServer4.WsFederation.WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString
                }
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0&wreply={client.BaseAddress}");
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.StartsWith("/Account/Login", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async Task Index_should_return_signin_document_when_user_found()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "Development",
                ["Seed"] = "true"
            };
            var userSessionMock = new Mock<IUserSession>();
            var sub = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("name", name),
                        new Claim("sub", sub),
                        new Claim("amr", Guid.NewGuid().ToString())
                    },
                    "wsfed",
                    "name",
                    "role"));
            userSessionMock.Setup(m => m.GetUserAsync()).ReturnsAsync(user);

            var sut = TestUtils.CreateTestServer(services =>
            {
                services.AddTransient(p => userSessionMock.Object);
            }, configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new Client
            {
                Id = clientId,
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation,
                AllowedScopes = new[]
                {
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Scope = "openid"
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Scope = "profile"
                    }
                },
                RelyingParty = new RelyingParty
                {
                    Id = clientId,
                    TokenType = IdentityServer4.WsFederation.WsFederationConstants.TokenTypes.Saml11TokenProfile11,
                    DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                    SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                    SamlNameIdentifierFormat = IdentityServer4.WsFederation.WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString,
                    ClaimMappings = new[]
                    {
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Name,
                            ToClaimType = ClaimTypes.Name
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Subject,
                            ToClaimType = ClaimTypes.NameIdentifier
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Email,
                            ToClaimType = ClaimTypes.Email
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.GivenName,
                            ToClaimType = ClaimTypes.GivenName
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.FamilyName,
                            ToClaimType = ClaimTypes.Surname
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.BirthDate,
                            ToClaimType = ClaimTypes.DateOfBirth
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.WebSite,
                            ToClaimType = ClaimTypes.Webpage
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Gender,
                            ToClaimType = ClaimTypes.Gender
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Role,
                            ToClaimType = ClaimTypes.Role
                        }
                    }
                }
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await identityContext.Users.AddAsync(new User
            {
                Id = sub,
                UserName = name,
                NormalizedUserName = name.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString()
            }).ConfigureAwait(false);
            await identityContext.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0&wreply={client.BaseAddress}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.NotNull(content);
        }

        [Fact]
        public async Task Index_should_return_signin_document_with_client_claim_when_user_found()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "Development",
                ["Seed"] = "true"
            };
            var sub = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(JwtClaimTypes.Name, name),
                        new Claim(JwtClaimTypes.Subject, sub),
                        new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password)
                    },
                    "wsfed",
                    "name",
                    "role"));

            var userSessionMock = new Mock<IUserSession>();
            userSessionMock.Setup(m => m.GetUserAsync()).ReturnsAsync(user);

            var profileServiceMock = new Mock<IProfileService>();
            profileServiceMock.Setup(m => m.GetProfileDataAsync(It.IsAny<ISModels.ProfileDataRequestContext>()))
                .Callback<ISModels.ProfileDataRequestContext>(ctx => ctx.IssuedClaims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Name, name),
                    new Claim(JwtClaimTypes.Subject, sub),
                    new Claim("http://exemple.com", Guid.NewGuid().ToString()),
                })
                .Returns(Task.CompletedTask);

            var sut = TestUtils.CreateTestServer(services =>
            {
                services.AddTransient(p => userSessionMock.Object)
                    .AddTransient(p => profileServiceMock.Object);
            }, configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new Client
            {
                Id = clientId,
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation,
                AllowedScopes = new[]
                {
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Scope = "openid"
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Scope = "profile"
                    }
                },
                ClientClaims = new[]
                {
                    new ClientClaim
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Type = "http://myorg.com/claim",
                        Value = Guid.NewGuid().ToString()
                    }
                },
                RelyingParty = new RelyingParty
                {
                    Id = clientId,
                    TokenType = IdentityServer4.WsFederation.WsFederationConstants.TokenTypes.Saml11TokenProfile11,
                    DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                    SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                    SamlNameIdentifierFormat = IdentityServer4.WsFederation.WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString,
                    ClaimMappings = new[]
                    {
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Name,
                            ToClaimType = ClaimTypes.Name
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Subject,
                            ToClaimType = ClaimTypes.NameIdentifier
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Email,
                            ToClaimType = ClaimTypes.Email
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.GivenName,
                            ToClaimType = ClaimTypes.GivenName
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.FamilyName,
                            ToClaimType = ClaimTypes.Surname
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.BirthDate,
                            ToClaimType = ClaimTypes.DateOfBirth
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.WebSite,
                            ToClaimType = ClaimTypes.Webpage
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Gender,
                            ToClaimType = ClaimTypes.Gender
                        },
                        new RelyingPartyClaimMapping
                        {
                            Id = Guid.NewGuid().ToString(),
                            RelyingPartyId = clientId,
                            FromClaimType = JwtClaimTypes.Role,
                            ToClaimType = ClaimTypes.Role
                        }
                    }
                }
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await identityContext.Users.AddAsync(new User
            {
                Id = sub,
                UserName = name,
                NormalizedUserName = name.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString()
            }).ConfigureAwait(false);
            await identityContext.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0&wreply={client.BaseAddress}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.NotNull(content);
            Assert.Contains("exemple.com", content);
            Assert.Contains("myorg.com", content);
        }

        [Fact]
        public async Task Index_should_return_signin_document_for_saml2_token_type_when_user_found()
        {
            var configuration = new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = "Development",
                ["Seed"] = "true"
            };
            var sub = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(JwtClaimTypes.Name, name),
                        new Claim(JwtClaimTypes.Subject, sub),
                        new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password)
                    },
                    "wsfed",
                    "name",
                    "role"));

            var userSessionMock = new Mock<IUserSession>();
            userSessionMock.Setup(m => m.GetUserAsync()).ReturnsAsync(user);

            var profileServiceMock = new Mock<IProfileService>();
            profileServiceMock.Setup(m => m.GetProfileDataAsync(It.IsAny<ISModels.ProfileDataRequestContext>()))
                .Callback<ISModels.ProfileDataRequestContext>(ctx => ctx.IssuedClaims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Name, name),
                    new Claim(JwtClaimTypes.Subject, sub),
                    new Claim("exemple.com", Guid.NewGuid().ToString()),
                })
                .Returns(Task.CompletedTask);

            var sut = TestUtils.CreateTestServer(services =>
            {
                services.AddTransient(p => userSessionMock.Object)
                    .AddTransient(p => profileServiceMock.Object);
            }, configuration);

            using var scope = sut.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            var clientId = $"urn:{Guid.NewGuid()}";
            await context.Clients.AddAsync(new Client
            {
                Id = clientId,
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation,
                AllowedScopes = new[]
                {
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Scope = "openid"
                    },
                    new ClientScope
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = clientId,
                        Scope = "profile"
                    }
                },
                RelyingParty = new RelyingParty
                {
                    Id = clientId,
                    TokenType = IdentityServer4.WsFederation.WsFederationConstants.TokenTypes.Saml2TokenProfile11,
                    DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                    SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                    SamlNameIdentifierFormat = IdentityServer4.WsFederation.WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString,
                    ClaimMappings = Array.Empty<RelyingPartyClaimMapping>()
                }
            }).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await identityContext.Users.AddAsync(new User
            {
                Id = sub,
                UserName = name,
                NormalizedUserName = name.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString()
            }).ConfigureAwait(false);
            await identityContext.SaveChangesAsync().ConfigureAwait(false);

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignin1.0&wreply={client.BaseAddress}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.NotNull(content);
            Assert.Contains("exemple.com", content);
        }


        [Fact]
        public async Task Index_should_redirect_to_logout_page_on_signout_message()
        {
            var configuration = new Dictionary<string, string>
            {
                ["Seed"] = "false"
            };
            var sut = TestUtils.CreateTestServer(configurationOverrides: configuration);

            var clientId = $"urn:{Guid.NewGuid()}";

            using var client = sut.CreateClient();
            using var response = await client.GetAsync($"/wsfederation?wtrealm={clientId}&wa=wsignout1.0&wreply={client.BaseAddress}");

            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.StartsWith("/connect/endsession", response.Headers.Location.OriginalString);
        }
    }
}