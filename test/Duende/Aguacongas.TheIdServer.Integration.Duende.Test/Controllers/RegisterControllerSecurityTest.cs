// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Options;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.IntegrationTest;
using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using IdentityModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.Integration.Duende.Test.Controllers;

/// <summary>
/// Dynamic client registration must not create clients without PKCE / client authentication
/// (GHSA-mw4v-v5j2-7f3g).
/// </summary>
[Collection(BlazorAppCollection.Name)]
public class RegisterControllerSecurityTest(TheIdServerFactory factory)
{
    [Theory]
    [InlineData("authorization_code")]
    [InlineData("authorization_code,refresh_token")]
    public async Task CreateAsync_should_require_pkce_and_client_secret_for_web_code_clients(string grantTypes)
    {
        var (status, result, _) = await RegisterAsync(NewRegistration("web", grantTypes, "https://localhost/cb"));

        Assert.Equal(HttpStatusCode.Created, status);
        Assert.NotNull(result.ClientSecret);

        var entity = await GetClientAsync(result.Id);
        Assert.True(entity.RequirePkce);
        Assert.True(entity.RequireClientSecret);
    }

    [Fact]
    public async Task CreateAsync_should_create_native_code_client_as_public_client_with_pkce()
    {
        var (status, result, _) = await RegisterAsync(NewRegistration("native", "authorization_code", "http://localhost"));

        Assert.Equal(HttpStatusCode.Created, status);

        var entity = await GetClientAsync(result.Id);
        Assert.True(entity.RequirePkce);
        Assert.False(entity.RequireClientSecret);
    }

    [Theory]
    [InlineData("web", "client_credentials")]
    [InlineData("native", "client_credentials")]
    [InlineData("native", "authorization_code,client_credentials")]
    public async Task CreateAsync_should_require_client_secret_when_client_is_not_a_native_code_only_client(string applicationType, string grantTypes)
    {
        var redirectUri = grantTypes.Contains("authorization_code") ? "http://localhost" : null;
        var (status, result, _) = await RegisterAsync(NewRegistration(applicationType, grantTypes, redirectUri));

        Assert.Equal(HttpStatusCode.Created, status);

        var entity = await GetClientAsync(result.Id);
        Assert.True(entity.RequireClientSecret);
    }

    [Theory]
    [InlineData("http://app.example.com/cb")]
    [InlineData("http://192.0.2.10/cb")]
    public async Task CreateAsync_should_reject_non_https_redirect_uri_for_web_code_clients(string redirectUri)
    {
        var (status, _, content) = await RegisterAsync(NewRegistration("web", "authorization_code", redirectUri));

        Assert.Equal(HttpStatusCode.BadRequest, status);

        var error = JsonConvert.DeserializeObject<RegistrationProblemDetail>(content);
        Assert.Equal("invalid_redirect_uri", error?.Error);
        Assert.Contains("authorization code flow", error?.Error_description);
    }

    [Theory]
    [InlineData("http://localhost")]
    [InlineData("http://127.0.0.1:5000/cb")]
    public async Task CreateAsync_should_accept_loopback_http_redirect_uri_for_web_code_clients(string redirectUri)
    {
        var (status, _, _) = await RegisterAsync(NewRegistration("web", "authorization_code", redirectUri));

        Assert.Equal(HttpStatusCode.Created, status);
    }

    [Fact]
    public async Task UpdateAsync_should_require_pkce_when_authorization_code_is_added_to_an_existing_client()
    {
        var (status, registration, _) = await RegisterAsync(NewRegistration("web", "implicit", "https://test"));
        Assert.Equal(HttpStatusCode.Created, status);

        var entity = await GetClientAsync(registration.Id);
        Assert.False(entity.RequirePkce);

        registration.GrantTypes = new[] { "authorization_code" };

        using var client = factory.CreateClient();
        using var content = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json");
        using var message = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            Content = content,
            RequestUri = new Uri(registration.RegistrationUri)
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registration.RegistrationToken);

        using var response = await client.SendAsync(message);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        entity = await GetClientAsync(registration.Id);
        Assert.True(entity.RequirePkce);
        Assert.True(entity.RequireClientSecret);
    }

    [Fact]
    public async Task CreateAsync_with_PublicClientsOnly_should_not_require_pkce_for_confidential_clients()
    {
        await WithOptionsAsync(o => o.RequirePkce = PkceRequirement.PublicClientsOnly, async () =>
        {
            var (status, web, _) = await RegisterAsync(NewRegistration("web", "authorization_code", "https://localhost/cb"));
            Assert.Equal(HttpStatusCode.Created, status);
            var entity = await GetClientAsync(web.Id);
            Assert.False(entity.RequirePkce);
            Assert.True(entity.RequireClientSecret);

            (status, var native, _) = await RegisterAsync(NewRegistration("native", "authorization_code", "http://localhost"));
            Assert.Equal(HttpStatusCode.Created, status);
            entity = await GetClientAsync(native.Id);
            Assert.True(entity.RequirePkce);
            Assert.False(entity.RequireClientSecret);
        });
    }

    [Fact]
    public async Task CreateAsync_without_client_authentication_and_PublicClientsOnly_should_require_pkce()
    {
        await WithOptionsAsync(o =>
        {
            o.RequireClientAuthentication = false;
            o.RequirePkce = PkceRequirement.PublicClientsOnly;
        }, async () =>
        {
            var (status, web, _) = await RegisterAsync(NewRegistration("web", "authorization_code", "https://localhost/cb"));
            Assert.Equal(HttpStatusCode.Created, status);
            var entity = await GetClientAsync(web.Id);
            Assert.False(entity.RequireClientSecret);
            Assert.True(entity.RequirePkce);
        });
    }

    [Fact]
    public async Task CreateAsync_should_accept_http_redirect_uri_when_https_is_not_required()
    {
        await WithOptionsAsync(o => o.RequireHttpsRedirectUris = false, async () =>
        {
            var (status, _, _) = await RegisterAsync(NewRegistration("web", "authorization_code", "http://app.example.com/cb"));
            Assert.Equal(HttpStatusCode.Created, status);
        });
    }

    [Fact]
    public async Task CreateAsync_should_reject_loopback_http_redirect_uri_when_loopback_is_not_allowed()
    {
        await WithOptionsAsync(o => o.AllowLoopbackHttpRedirectUris = false, async () =>
        {
            var (status, _, _) = await RegisterAsync(NewRegistration("web", "authorization_code", "http://localhost"));
            Assert.Equal(HttpStatusCode.BadRequest, status);
        });
    }

    // Options are a singleton shared by the test host: change them for one test and always restore them.
    private async Task WithOptionsAsync(Action<DynamicClientRegistrationOptions> change, Func<Task> test)
    {
        var options = factory.Services.GetRequiredService<IOptions<DynamicClientRegistrationOptions>>().Value;
        var previous = (options.RequirePkce, options.RequireClientAuthentication, options.RequireHttpsRedirectUris, options.AllowLoopbackHttpRedirectUris);
        try
        {
            change(options);
            await test();
        }
        finally
        {
            (options.RequirePkce, options.RequireClientAuthentication, options.RequireHttpsRedirectUris, options.AllowLoopbackHttpRedirectUris) = previous;
        }
    }

    private static ClientRegisteration NewRegistration(string applicationType, string grantTypes, string redirectUri)
        => new ClientRegisteration
        {
            ApplicationType = applicationType,
            GrantTypes = grantTypes.Split(','),
            ClientNames = new List<LocalizableProperty>
            {
                new LocalizableProperty
                {
                    Value = $"security-{Guid.NewGuid():N}"
                }
            },
            RedirectUris = redirectUri == null ? Array.Empty<string>() : new[] { redirectUri }
        };

    private async Task<(HttpStatusCode Status, ClientRegisteration Registration, string Content)> RegisterAsync(ClientRegisteration registration)
    {
        factory.Services.GetRequiredService<TestUserService>()
                .SetTestUser(true, new Claim[]
                {
                    new Claim(JwtClaimTypes.Role, SharedConstants.WRITERPOLICY),
                    new Claim(JwtClaimTypes.Scope, SharedConstants.ADMINSCOPE)
                });

        using var client = factory.CreateClient();
        using var request = new StringContent(JsonConvert.SerializeObject(registration), Encoding.UTF8, "application/json");
        using var response = await client.PostAsync("/api/register", request);

        var content = await response.Content.ReadAsStringAsync();
        var result = response.StatusCode == HttpStatusCode.Created
            ? JsonConvert.DeserializeObject<ClientRegisteration>(content)
            : null;

        return (response.StatusCode, result, content);
    }

    private async Task<Entity.Client> GetClientAsync(string clientId)
    {
        using var scope = factory.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IAdminStore<Entity.Client>>();
        return await store.GetAsync(clientId, null);
    }
}