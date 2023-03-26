using Aguacongas.IdentityServer.Store;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Security.Cryptography.X509Certificates;
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
        var client = await _clientStore.GetAsync(issuer, null).ConfigureAwait(false);

        var logoutUri = client.RedirectUris.FirstOrDefault(u => u.Kind == Entity.UriKinds.PostLogout)?.Uri;
        var acsUri = client.RedirectUris.FirstOrDefault(u => u.Kind == Entity.UriKinds.Acs)?.Uri;
        var relyingParty = new RelyingParty
        {
            Issuer = issuer,
            UseAcsArtifact = client.UseAcsArtifact,
            AcsDestination = acsUri is not null ? new Uri(acsUri) : null,
            SingleLogoutDestination = logoutUri is not null ? new Uri(logoutUri) : null,
            SignatureValidationCertificate = client.SignatureValidationCertificate is not null ? new X509Certificate2(client.SignatureValidationCertificate) : null
        };
        var metadata = client.Saml2PMetadata;
        if (metadata != null)
        {
            return await LoadRelyingPartyFromMetadataAsync(relyingParty, metadata).ConfigureAwait(false);
        }
        return relyingParty;
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
        relyingParty.SignatureValidationCertificate = entityDescriptor.SPSsoDescriptor.SigningCertificates.First();
        return relyingParty;
    }
}
