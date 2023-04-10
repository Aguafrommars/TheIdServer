using ITfoxtec.Identity.Saml2;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;

/// <summary>
/// Saml2P configuration service interface
/// </summary>
public interface ISaml2ConfigurationService
{
    /// <summary>
    /// Gets the configuration
    /// </summary>
    /// <returns>a <see cref="Saml2Configuration"/></returns>
    Task<Saml2Configuration> GetConfigurationAsync();
}