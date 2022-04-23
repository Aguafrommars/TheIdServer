using Aguacongas.IdentityServer.Shared.Validators;
#if DUENDE
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
#else
using IdentityServer4.Models;
using IdentityServer4.Validation;
#endif
using IdentityModel;
using Moq;
using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Test.Shared.Validators
{
    public class TokenExchangeGrantValidatorTest
    {
        [Fact]
        public void Constructor_should_verify_paramenters()
        {
            Assert.Throws<ArgumentNullException>(() => new TokenExchangeGrantValidator(null));
        }

        [Fact]
        public async Task ValidateAsync_should_verify_subject_token()
        {
            var sut = new TokenExchangeGrantValidator(new Mock<ITokenValidator>().Object);
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection()
                }
            };
            await sut.ValidateAsync(context).ConfigureAwait(false);

            Assert.Equal("invalid_request", context.Result.Error);

            context.Request.Raw.Add(OidcConstants.TokenRequest.SubjectToken, "test");

            await sut.ValidateAsync(context).ConfigureAwait(false);

            Assert.Equal("invalid_request", context.Result.Error);
        }

        [Fact]
        public async Task ValidateAsync_should_validate_token_using_validator()
        {
            var validatorMock = new Mock<ITokenValidator>();
            validatorMock.Setup(m => m.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokenValidationResult
            {
                IsError = true
            });

            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection
                    {
                        [OidcConstants.TokenRequest.SubjectToken] = "test",
                        [OidcConstants.TokenRequest.SubjectTokenType] = OidcConstants.TokenTypeIdentifiers.AccessToken,
                    }
                }
            };

            var sut = new TokenExchangeGrantValidator(validatorMock.Object);
            await sut.ValidateAsync(context).ConfigureAwait(false);

            Assert.Equal("invalid_request", context.Result.Error);
        }

        [Fact]
        public async Task ValidateAsync_should_create_impersonation_result()
        {
            var validatorMock = new Mock<ITokenValidator>();
            validatorMock.Setup(m => m.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokenValidationResult
            {
                IsError = false,
                Claims = new Claim[]
                {
                    new Claim(JwtClaimTypes.Subject, "test"),
                    new Claim(JwtClaimTypes.ClientId, "test")
                }
            });

            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection
                    {
                        [OidcConstants.TokenRequest.SubjectToken] = "test",
                        [OidcConstants.TokenRequest.SubjectTokenType] = OidcConstants.TokenTypeIdentifiers.AccessToken,
                        ["exchange_style"] = "impersonation"
                    }
                }
            };

            var sut = new TokenExchangeGrantValidator(validatorMock.Object);
            await sut.ValidateAsync(context).ConfigureAwait(false);

            Assert.False(context.Result.IsError);
            Assert.Equal("test", context.Request.ClientId);
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.Subject && c.Value == "test");
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.AuthenticationMethod && c.Value == OidcConstants.GrantTypes.TokenExchange);
        }

        [Fact]
        public async Task ValidateAsync_should_create_delegation_result()
        {
            var validatorMock = new Mock<ITokenValidator>();
            validatorMock.Setup(m => m.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokenValidationResult
            {
                IsError = false,
                Claims = new Claim[]
                {
                    new Claim(JwtClaimTypes.Subject, "test"),
                    new Claim(JwtClaimTypes.ClientId, "test")
                }
            });

            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection
                    {
                        [OidcConstants.TokenRequest.SubjectToken] = "test",
                        [OidcConstants.TokenRequest.SubjectTokenType] = OidcConstants.TokenTypeIdentifiers.AccessToken,
                        ["exchange_style"] = "delegation"
                    },
                    Client = new Client
                    {
                        ClientId = "test"
                    }
                }
            };

            var sut = new TokenExchangeGrantValidator(validatorMock.Object);
            await sut.ValidateAsync(context).ConfigureAwait(false);

            Assert.False(context.Result.IsError);
            Assert.Equal("test", context.Request.ClientId);
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.Subject && c.Value == "test");
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.AuthenticationMethod && c.Value == OidcConstants.GrantTypes.TokenExchange);
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.Actor && c.Value == "{\"client_id\":\"test\"}");
        }

        [Fact]
        public async Task ValidateAsync_should_create_custom_result()
        {
            var validatorMock = new Mock<ITokenValidator>();
            validatorMock.Setup(m => m.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokenValidationResult
            {
                IsError = false,
                Claims = new Claim[]
                {
                    new Claim(JwtClaimTypes.Subject, "test"),
                    new Claim(JwtClaimTypes.ClientId, "test")
                }
            });

            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection
                    {
                        [OidcConstants.TokenRequest.SubjectToken] = "test",
                        [OidcConstants.TokenRequest.SubjectTokenType] = OidcConstants.TokenTypeIdentifiers.AccessToken,
                        ["exchange_style"] = "custom"
                    },
                    Client = new Client
                    {
                        ClientId = "test"
                    }
                }
            };

            var sut = new TokenExchangeGrantValidator(validatorMock.Object);
            await sut.ValidateAsync(context).ConfigureAwait(false);

            Assert.False(context.Result.IsError);
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.Subject && c.Value == "test");
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == JwtClaimTypes.AuthenticationMethod && c.Value == OidcConstants.GrantTypes.TokenExchange);
        }
    }
}
