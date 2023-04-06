using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer.Services;
using Duende.IdentityServer;
using IdentityModel;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Aguacongas.TheIdServer.Integration.Duende.Test.Controlers;

[Collection(BlazorAppCollection.Name)]
public class Saml2PControllerTest
{
    private WebApplicationFactory<AccountController> _factory;

    public Saml2PControllerTest(TheIdServerFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Metadata_should_return_saml2_metadata()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/saml2p/metadata");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var descriptor = new EntityDescriptor();
        descriptor.ReadIdPSsoDescriptor(content);

        Assert.NotNull(descriptor.IdPSsoDescriptor);
    }

    [Fact]
    public async Task Login_should_return_saml2_result_when_user_user_found()
    {
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

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        var clientId = $"urn:{Guid.NewGuid()}";
        await context.Clients.AddAsync(new Client
        {
            Id = clientId,
            Enabled = true,
            ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
            RedirectUris = new[]
            {
                new ClientUri
                {
                    
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


        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/saml2p/metadata");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var descriptor = new EntityDescriptor();
        descriptor.ReadIdPSsoDescriptor(content);

        Assert.NotNull(descriptor.IdPSsoDescriptor);
    }
}
