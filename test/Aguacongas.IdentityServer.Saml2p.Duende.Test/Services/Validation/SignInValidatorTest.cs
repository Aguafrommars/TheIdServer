using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Test.Services.Validation;
public class SignInValidatorTest
{
    [Fact]
    public async Task ValidateArtifactRequestAsync_should_validate_relying_party()
    {
        var clientStoreMock = new Mock<IClientStore>();
        clientStoreMock.Setup(m => m.FindClientByIdAsync(It.IsAny<string>())).ReturnsAsync(new Client
        {
            Enabled = true,
            ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p
        });

        var relyingPartyStoreMock = new Mock<IRelyingPartyStore>();
        var samlConfigurationServiceMock = new Mock<ISaml2ConfigurationService>();

        var sut = new SignInValidator(clientStoreMock.Object, 
            relyingPartyStoreMock.Object, 
            samlConfigurationServiceMock.Object);

        var httpRequestMock = new Mock<HttpRequest>();
        var result = await sut.ValidateArtifactRequestAsync(httpRequestMock.Object).ConfigureAwait(false);
        Assert.NotNull(result.Error);
    }
}
