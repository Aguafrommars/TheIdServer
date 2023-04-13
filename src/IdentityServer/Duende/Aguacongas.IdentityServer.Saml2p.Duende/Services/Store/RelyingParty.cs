using System.Security.Cryptography.X509Certificates;
using static Microsoft.IdentityModel.Tokens.Saml2.Saml2Constants;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;

/// <summary>
/// Relying party
/// </summary>
public class RelyingParty
{
    /// <summary>
    /// Metadata url
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Issuer
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// Assertion Consumer Service destination
    /// </summary>
    public Uri? AcsDestination { get; set; }

    /// <summary>
    /// Use Assertion Consumer Service artifact
    /// </summary>
    public bool UseAcsArtifact { get; set; } = false;

    /// <summary>
    /// Single logout destination
    /// </summary>
    public Uri? SingleLogoutDestination { get; set; }

    /// <summary>
    /// Signature validation certificate list
    /// </summary>
    public IEnumerable<X509Certificate2?> SignatureValidationCertificate { get; set; } = Array.Empty<X509Certificate2>();

    /// <summary>
    /// Encryption certificate
    /// </summary>
    public X509Certificate2? EncryptionCertificate { get; set; }
    
    /// <summary>
    /// Signature algorithm
    /// </summary>
    public string? SignatureAlgorithm { get; set; }

    /// <summary>
    /// Saml name identifier format
    /// </summary>
    public Uri SamlNameIdentifierFormat { get; set; } = NameIdentifierFormats.Persistent;

    /// <summary>
    /// Claims mapping
    /// </summary>
    public IDictionary<string, string> ClaimMapping { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Token type
    /// </summary>
    public string? TokenType { get; set; }
}