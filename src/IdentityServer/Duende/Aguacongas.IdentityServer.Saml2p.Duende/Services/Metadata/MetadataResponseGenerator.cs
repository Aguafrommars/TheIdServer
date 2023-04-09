using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Metadata;

/// <summary>
/// Metadata response generator
/// </summary>
public class MetadataResponseGenerator : IMetadataResponseGenerator
{
    private readonly ISaml2ConfigurationService _saml2ConfigurationService;
    private readonly IOptions<Saml2POptions> _options;

    /// <summary>
    /// Initialize a new instance of <see cref="MetadataResponseGenerator"/>
    /// </summary>
    /// <param name="saml2ConfigurationService"></param>
    /// <param name="options"></param>
    public MetadataResponseGenerator(ISaml2ConfigurationService saml2ConfigurationService,
        IOptions<Saml2POptions> options)
    {
        _saml2ConfigurationService = saml2ConfigurationService;
        _options = options;
    }

    /// <summary>
    /// Generates the metadata response
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> GenerateMetadataResponseAsync()
    {
        var config = await _saml2ConfigurationService.GetConfigurationAsync().ConfigureAwait(false);
        var settings = _options.Value;

        var entityDescriptor = new EntityDescriptor(config)
        {
            ValidUntil = settings.ValidUntil,
            IdPSsoDescriptor = new IdPSsoDescriptor
            {
                SigningCertificates = new[] { config.SigningCertificate },
                SingleSignOnServices = new SingleSignOnService[]
                {
                    new SingleSignOnService 
                    { 
                        Binding = ProtocolBindings.HttpRedirect, 
                        Location = config.SingleSignOnDestination 
                    }
                },
                SingleLogoutServices = new SingleLogoutService[]
                {
                    new SingleLogoutService 
                    { 
                        Binding = ProtocolBindings.HttpPost, 
                        Location = config.SingleLogoutDestination 
                    }
                },
                ArtifactResolutionServices = new ArtifactResolutionService[]
                {
                    new ArtifactResolutionService 
                    {
                        Binding = ProtocolBindings.ArtifactSoap, 
                        Index = config.ArtifactResolutionService.Index, 
                        Location = config.ArtifactResolutionService.Location 
                    }
                },
                NameIDFormats = new Uri[] { NameIdentifierFormats.X509SubjectName },
            },
            ContactPersons = settings.ContactPersons
        };

        return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
    }
}
