using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
public class RelyingParty
{
    public string? Metadata { get; set; }

    public string? Issuer { get; set; }

    public Uri? AcsDestination { get; set; }

    public bool UseAcsArtifact { get; set; } = false;

    public Uri? SingleLogoutDestination { get; set; }

    public IEnumerable<X509Certificate2?> SignatureValidationCertificate { get; set; } = Array.Empty<X509Certificate2>();

    public X509Certificate2? EncryptionCertificate { get; set; }
}