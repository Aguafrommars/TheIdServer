using Aguacongas.IdentityServer.Saml2p.Duende.Services;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ServiceModel.Security;

namespace Aguacongas.IdentityServer.Saml2P.Duende.Test.Extensions;
public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddIdentityServerSaml2P_should_add_saml2p_services_in_di()
    {
        var builder = new ServiceCollection()
            .AddMvc()
            .AddIdentityServerSaml2P(new Saml2POptions
            {
                CertificateValidationMode = X509CertificateValidationMode.None
            });

        Assert.Contains(builder.Services, s => s.ServiceType == typeof(ISaml2PService));
    }
}
