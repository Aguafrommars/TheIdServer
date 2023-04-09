using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using ITfoxtec.Identity.Saml2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;

/// <summary>
/// Saml2P configuration service
/// </summary>
public class Saml2ConfigurationService : ISaml2ConfigurationService
{
    private readonly ISigningCredentialStore _signingCredentialStore;
    private readonly IOptions<Saml2POptions> _options;
    private readonly IIssuerNameService _issuerNameService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initialize a new instance of <see cref="Saml2ConfigurationService"/>
    /// </summary>
    /// <param name="signingCredentialStore"></param>
    /// <param name="issuerNameService"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="options"></param>
    public Saml2ConfigurationService(ISigningCredentialStore signingCredentialStore,
        IIssuerNameService issuerNameService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<Saml2POptions> options)
    {
        _signingCredentialStore = signingCredentialStore;
        _issuerNameService = issuerNameService;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }

    /// <summary>
    /// Gets the configuration
    /// </summary>
    /// <returns>a <see cref="Saml2Configuration"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Saml2Configuration> GetConfigurationAsync()
    {
        var request = (_httpContextAccessor.HttpContext?.Request) ?? throw new InvalidOperationException("Http request cannot be null");
        var location = Location(request);
        var settings = _options.Value;
        var credentials = await _signingCredentialStore.GetSigningCredentialsAsync().ConfigureAwait(false);
        
        return new Saml2Configuration
        {
            ArtifactResolutionService = new Saml2IndexedEndpoint
            {
                Index = 1,
                Location = new Uri($"{location}/artifact")
            },
            SingleSignOnDestination = new Uri($"{location}/login"),
            SingleLogoutDestination = new Uri($"{location}/logout"),
            Issuer = await _issuerNameService.GetCurrentAsync().ConfigureAwait(false),
            SignatureAlgorithm = settings.SignatureAlgorithm,
            SigningCertificate = credentials.Key.GetX509Certificate(_signingCredentialStore),
            CertificateValidationMode = settings.CertificateValidationMode,
            RevocationMode = settings.RevocationMode
        };
    }

    private static string Location(HttpRequest request)
    => $"{request.Scheme}://{request.Host}/saml2p";
}
