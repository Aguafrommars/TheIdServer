using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;

/// <summary>
/// Saml2P options
/// </summary>
public class Saml2POptions
{
    /// <summary>
    /// Certificate validatio mode
    /// </summary>
    public X509CertificateValidationMode CertificateValidationMode { get; set; }
    
    /// <summary>
    /// Contact person list
    /// </summary>
    public IEnumerable<Saml2PContactPerson>? ContactPersons { get; set; }
    
    /// <summary>
    /// Revocation mode
    /// </summary>
    public X509RevocationMode RevocationMode { get; set; }
    
    /// <summary>
    /// Signature algorithm
    /// </summary>
    public string? SignatureAlgorithm { get; set; }
    
    /// <summary>
    /// Metadata entity descriptor lifetime in day
    /// </summary>
    public int? ValidUntil { get; set; } = 365;
}