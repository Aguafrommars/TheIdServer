using ITfoxtec.Identity.Saml2;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;

public interface ISaml2ConfigurationService
{
    Task<Saml2Configuration> GetConfigurationAsync();
}