using Aguacongas.IdentityServer.Store;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Security.Cryptography.X509Certificates;
using static Duende.IdentityServer.IdentityServerConstants;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
public class RelyingPartyStore : IRelyingPartyStore
{
    private readonly IAdminStore<Entity.Client> _clientStore;
    private readonly IHttpClientFactory _httpClientFactory;

    public RelyingPartyStore(IAdminStore<Entity.Client> clientStore, IHttpClientFactory httpClientFactory)
    {
        _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<RelyingParty> FindRelyingPartyAsync(string issuer)
    {
        var client = await _clientStore.GetAsync(issuer, new GetRequest
        {
            Expand = $"{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.RedirectUris)}"
        }).ConfigureAwait(false);

        var logoutUri = client.RedirectUris.FirstOrDefault(u => u.Kind == Entity.UriKinds.PostLogout)?.Uri;
        var acsUri = client.RedirectUris.FirstOrDefault(u => u.Kind == Entity.UriKinds.Acs)?.Uri;
        var signatureCertificateList = client.ClientSecrets
            .Where(s => !s.UsedForEncryption && 
                (s.Expiration == null || s.Expiration > DateTime.UtcNow) &&
                (s.Type == SecretTypes.X509CertificateThumbprint ||
                s.Type == SecretTypes.X509CertificateName ||
                s.Type == SecretTypes.X509CertificateBase64))
            .Select(s => s.Type switch
            {
                SecretTypes.X509CertificateThumbprint => GetCertificateFromThumprint(s.Value),
                SecretTypes.X509CertificateName => GetCertificateFromName(s.Value),
                SecretTypes.X509CertificateBase64 => GetCertificateFromBase64(s.Value),
                _ => null
            });

        var encriptionsCertificate = client.ClientSecrets
            .Where(s => s.UsedForEncryption &&
                (s.Expiration == null || s.Expiration > DateTime.UtcNow) &&
                (s.Type == SecretTypes.X509CertificateThumbprint ||
                s.Type == SecretTypes.X509CertificateName ||
                s.Type == SecretTypes.X509CertificateBase64))
            .OrderBy(s => s.Expiration)
            .Select(s => s.Type switch
            {
                SecretTypes.X509CertificateThumbprint => GetCertificateFromThumprint(s.Value),
                SecretTypes.X509CertificateName => GetCertificateFromName(s.Value),
                SecretTypes.X509CertificateBase64 => GetCertificateFromBase64(s.Value),
                _ => null
            })
            .FirstOrDefault();

        var relyingParty = new RelyingParty
        {
            Issuer = issuer,
            UseAcsArtifact = client.UseAcsArtifact,
            AcsDestination = acsUri is not null ? new Uri(acsUri) : null,
            SingleLogoutDestination = logoutUri is not null ? new Uri(logoutUri) : null,
            SignatureValidationCertificate = signatureCertificateList,
            EncryptionCertificate = encriptionsCertificate
        };
        var metadata = client.Saml2PMetadata;
        if (metadata != null)
        {
            return await LoadRelyingPartyFromMetadataAsync(relyingParty, metadata).ConfigureAwait(false);
        }
        return relyingParty;
    }

    private static X509Certificate2 GetCertificateFromBase64(string value)
    => new X509Certificate2(Convert.FromBase64String(value));

    private static X509Certificate2? GetCertificateFromName(string value)
    => FindCertificate(value, X509FindType.FindBySubjectDistinguishedName);
    private static X509Certificate2? GetCertificateFromThumprint(string value)
    => FindCertificate(value, X509FindType.FindByThumbprint);

    private static X509Certificate2? FindCertificate(string value, X509FindType findType)
    {
        foreach (var location in Enum.GetValues<StoreLocation>())
        {
            foreach (var name in Enum.GetValues<StoreName>())
            {
                using var store = new X509Store();
                var certificate = FindCertificate(value, findType, store);
                if (certificate is not null)
                {
                    return certificate;
                }
            }
        }

        return null;
    }

    private static X509Certificate2? FindCertificate(string value, 
        X509FindType findType, 
        X509Store store)
    {
        store.Open(OpenFlags.ReadOnly);
        return store.Certificates.Find(findType, value, false)
            .FirstOrDefault();
    }

    private async Task<RelyingParty> LoadRelyingPartyFromMetadataAsync(RelyingParty relyingParty, string metadata)
    {
        var entityDescriptor = new EntityDescriptor();
        await entityDescriptor.ReadSPSsoDescriptorFromUrlAsync(_httpClientFactory, new Uri(metadata), default);

        if (entityDescriptor.SPSsoDescriptor == null)
        {
            throw new InvalidOperationException($"SPSsoDescriptor not loaded from metadata '{relyingParty.Metadata}'.");
        }

        relyingParty.Issuer = entityDescriptor.EntityId;
        relyingParty.Metadata = metadata;
        relyingParty.AcsDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices.Where(a => a.IsDefault).OrderBy(a => a.Index).First().Location;
        var singleLogoutService = entityDescriptor.SPSsoDescriptor.SingleLogoutServices.First();
        relyingParty.SingleLogoutDestination = singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
        relyingParty.SignatureValidationCertificate = entityDescriptor.SPSsoDescriptor.SigningCertificates;
        return relyingParty;
    }
}
