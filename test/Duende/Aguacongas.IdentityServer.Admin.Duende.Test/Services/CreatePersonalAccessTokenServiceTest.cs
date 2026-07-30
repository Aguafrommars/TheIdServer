// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Services.Test;

public class CreatePersonalAccessTokenServiceTest
{
    private const string ClientId = "test-client";
    private const string SubjectId = "test-subject";
    private const string UserName = "test-user";

    [Fact]
    public void Constructor_should_validate_parameters()
    {
        var issuerNameServiceMock = new Mock<IIssuerNameService>();
        var tokenServiceMock = new Mock<ITokenService>();
        var clientStoreMock = new Mock<IClientStore>();
        var resourceStoreMock = new Mock<IResourceStore>();

        Assert.Throws<ArgumentNullException>(() => new CreatePersonalAccessTokenService(null, tokenServiceMock.Object, clientStoreMock.Object, resourceStoreMock.Object));
        Assert.Throws<ArgumentNullException>(() => new CreatePersonalAccessTokenService(issuerNameServiceMock.Object, null, clientStoreMock.Object, resourceStoreMock.Object));
        Assert.Throws<ArgumentNullException>(() => new CreatePersonalAccessTokenService(issuerNameServiceMock.Object, tokenServiceMock.Object, null, resourceStoreMock.Object));
        Assert.Throws<ArgumentNullException>(() => new CreatePersonalAccessTokenService(issuerNameServiceMock.Object, tokenServiceMock.Object, clientStoreMock.Object, null));
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_throw_when_apis_is_null()
    {
        var sut = CreateSut(out _, out _, out _, out _);
        var context = CreateHttpContext();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.CreatePersonalAccessTokenAsync(context, false, 30, null, ["scope1"], []));
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_throw_when_client_not_found()
    {
        var sut = CreateSut(out _, out _, out var clientStoreMock, out _);
        clientStoreMock.Setup(s => s.FindClientByIdAsync(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client)null);

        var context = CreateHttpContext();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CreatePersonalAccessTokenAsync(context, false, 30, ["api1"], [], []));

        Assert.Contains($"Client not found for client id '{ClientId}'", exception.Message);
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_throw_when_api_not_found()
    {
        var sut = CreateSut(out _, out _, out var clientStoreMock, out var resourceStoreMock);
        clientStoreMock.Setup(s => s.FindClientByIdAsync(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client { ClientId = ClientId, AllowedScopes = new HashSet<string>() });
        resourceStoreMock.Setup(s => s.FindApiScopesByNameAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var context = CreateHttpContext();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CreatePersonalAccessTokenAsync(context, false, 30, ["api1"], [], []));

        Assert.Contains("Apis 'api1' not found.", exception.Message);
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_throw_when_scope_not_allowed()
    {
        var sut = CreateSut(out _, out _, out var clientStoreMock, out var resourceStoreMock);
        clientStoreMock.Setup(s => s.FindClientByIdAsync(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client { ClientId = ClientId, AllowedScopes = new HashSet<string> { "allowed-scope" } });
        resourceStoreMock.Setup(s => s.FindApiScopesByNameAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ApiScope { Name = "api1" }]);

        var context = CreateHttpContext();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CreatePersonalAccessTokenAsync(context, false, 30, ["api1"], ["not-allowed-scope"], []));

        Assert.Contains("Scopes 'not-allowed-scope' not found in", exception.Message);
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_create_reference_token_with_expected_claims()
    {
        var sut = CreateSut(out var issuerNameServiceMock, out var tokenServiceMock, out var clientStoreMock, out var resourceStoreMock);

        clientStoreMock.Setup(s => s.FindClientByIdAsync(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client { ClientId = ClientId, AllowedScopes = new HashSet<string> { "scope1" } });
        resourceStoreMock.Setup(s => s.FindApiScopesByNameAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ApiScope { Name = "api1" }]);
        issuerNameServiceMock.Setup(s => s.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://issuer.example.com");

        Token capturedToken = null;
        tokenServiceMock.Setup(s => s.CreateSecurityTokenAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>()))
            .Callback<Token, CancellationToken>((t, _) => capturedToken = t)
            .ReturnsAsync("the-token-value");

        var context = CreateHttpContext(additionalClaims:
        [
            new Claim("department", "engineering")
        ]);

        var beforeCall = DateTime.UtcNow;
        var result = await sut.CreatePersonalAccessTokenAsync(context, true, 30, ["api1"], ["scope1"], ["department"]);

        Assert.Equal("the-token-value", result);
        Assert.NotNull(capturedToken);
        Assert.Equal(AccessTokenType.Reference, capturedToken.AccessTokenType);
        Assert.Equal(["api1"], capturedToken.Audiences);
        Assert.Equal(ClientId, capturedToken.ClientId);
        Assert.Equal("https://issuer.example.com", capturedToken.Issuer);
        Assert.Equal(30 * 24 * 60 * 60, capturedToken.Lifetime);
        Assert.True(capturedToken.CreationTime >= beforeCall.AddSeconds(-1));

        Assert.Contains(capturedToken.Claims, c => c.Type == JwtClaimTypes.Name && c.Value == UserName);
        Assert.Contains(capturedToken.Claims, c => c.Type == JwtClaimTypes.ClientId && c.Value == ClientId);
        Assert.Contains(capturedToken.Claims, c => c.Type == JwtClaimTypes.Subject && c.Value == SubjectId);
        Assert.Contains(capturedToken.Claims, c => c.Type == "department" && c.Value == "engineering");
        Assert.Contains(capturedToken.Claims, c => c.Type == "scope" && c.Value == "scope1");

        // No duplicate Name/ClientId/Subject claims, even though the user principal already carries them
        Assert.Single(capturedToken.Claims, c => c.Type == JwtClaimTypes.Name);
        Assert.Single(capturedToken.Claims, c => c.Type == JwtClaimTypes.ClientId);
        Assert.Single(capturedToken.Claims, c => c.Type == JwtClaimTypes.Subject);
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_create_jwt_token_when_isRefenceToken_is_false()
    {
        var sut = CreateSut(out var issuerNameServiceMock, out var tokenServiceMock, out var clientStoreMock, out var resourceStoreMock);

        clientStoreMock.Setup(s => s.FindClientByIdAsync(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client { ClientId = ClientId, AllowedScopes = new HashSet<string>() });
        resourceStoreMock.Setup(s => s.FindApiScopesByNameAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ApiScope { Name = "api1" }]);
        issuerNameServiceMock.Setup(s => s.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://issuer.example.com");

        Token capturedToken = null;
        tokenServiceMock.Setup(s => s.CreateSecurityTokenAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>()))
            .Callback<Token, CancellationToken>((t, _) => capturedToken = t)
            .ReturnsAsync("jwt-value");

        var context = CreateHttpContext();

        await sut.CreatePersonalAccessTokenAsync(context, false, 1, ["api1"], [], []);

        Assert.Equal(AccessTokenType.Jwt, capturedToken.AccessTokenType);
    }

    [Fact]
    public async Task CreatePersonalAccessTokenAsync_should_exclude_name_clientid_and_subject_from_requested_claim_types()
    {
        var sut = CreateSut(out var issuerNameServiceMock, out var tokenServiceMock, out var clientStoreMock, out var resourceStoreMock);

        clientStoreMock.Setup(s => s.FindClientByIdAsync(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Client { ClientId = ClientId, AllowedScopes = new HashSet<string>() });
        resourceStoreMock.Setup(s => s.FindApiScopesByNameAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ApiScope { Name = "api1" }]);
        issuerNameServiceMock.Setup(s => s.GetCurrentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://issuer.example.com");

        Token capturedToken = null;
        tokenServiceMock.Setup(s => s.CreateSecurityTokenAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>()))
            .Callback<Token, CancellationToken>((t, _) => capturedToken = t)
            .ReturnsAsync("token");

        var context = CreateHttpContext();

        // Requesting Name/ClientId/Subject explicitly should not create duplicates,
        // since the service always filters them out of the caller-supplied claim types.
        await sut.CreatePersonalAccessTokenAsync(context, false, 1, ["api1"], [],
            [JwtClaimTypes.Name, JwtClaimTypes.ClientId, JwtClaimTypes.Subject]);

        Assert.Single(capturedToken.Claims, c => c.Type == JwtClaimTypes.Name);
        Assert.Single(capturedToken.Claims, c => c.Type == JwtClaimTypes.ClientId);
        Assert.Single(capturedToken.Claims, c => c.Type == JwtClaimTypes.Subject);
    }

    private static CreatePersonalAccessTokenService CreateSut(
        out Mock<IIssuerNameService> issuerNameServiceMock,
        out Mock<ITokenService> tokenServiceMock,
        out Mock<IClientStore> clientStoreMock,
        out Mock<IResourceStore> resourceStoreMock)
    {
        issuerNameServiceMock = new Mock<IIssuerNameService>();
        tokenServiceMock = new Mock<ITokenService>();
        clientStoreMock = new Mock<IClientStore>();
        resourceStoreMock = new Mock<IResourceStore>();

        return new CreatePersonalAccessTokenService(
            issuerNameServiceMock.Object,
            tokenServiceMock.Object,
            clientStoreMock.Object,
            resourceStoreMock.Object);
    }

    private static DefaultHttpContext CreateHttpContext(IEnumerable<Claim> additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Subject, SubjectId),
            new(JwtClaimTypes.ClientId, ClientId),
            new(JwtClaimTypes.Name, UserName)
        };

        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims);
        }

        var identity = new ClaimsIdentity(claims, "Bearer", JwtClaimTypes.Name, JwtClaimTypes.Role);

        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };
    }
}