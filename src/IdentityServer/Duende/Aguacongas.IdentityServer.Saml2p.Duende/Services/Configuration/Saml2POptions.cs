using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;

public class Saml2POptions
{
    public X509CertificateValidationMode CertificateValidationMode { get; set; }
    public IEnumerable<Saml2PContactPerson>? ContactPersons { get; set; }
    public X509RevocationMode RevocationMode { get; set; }
    public string? SignatureAlgorithm { get; set; }
    public int? ValidUntil { get; set; } = 365;
}