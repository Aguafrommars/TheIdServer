using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using Aguacongas.TheIdServer.UI;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
}
