﻿using Aguacongas.IdentityServer.Store;
using ITfoxtec.Identity.Saml2.Cryptography;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Security.Cryptography.X509Certificates;
using static Duende.IdentityServer.IdentityServerConstants;
using static Microsoft.IdentityModel.Tokens.Saml2.Saml2Constants;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;

/// <summary>
/// Relying party store
/// </summary>
public class RelyingPartyStore : IRelyingPartyStore
{
    private readonly IAdminStore<Entity.Client> _clientStore;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initialize a new instance of <see cref="RelyingPartyStore"/>
    /// </summary>
    /// <param name="clientStore"></param>
    /// <param name="httpClientFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RelyingPartyStore(IAdminStore<Entity.Client> clientStore, IHttpClientFactory httpClientFactory)
    {
        _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <summary>
    /// Find relying party for by issuer.
    /// </summary>
    /// <param name="issuer"></param>
    /// <returns></returns>
    public async Task<RelyingParty?> FindRelyingPartyAsync(string issuer)
    {
        var relyingPartyName = nameof(Entity.Client.RelyingParty);
        var relyingPartyNameClaimMappingName = nameof(Entity.Client.RelyingParty.ClaimMappings);
        var client = await _clientStore.GetAsync(issuer, new GetRequest
        {
            Expand = $"{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.RedirectUris)},{relyingPartyName},{relyingPartyName}.{relyingPartyNameClaimMappingName},{nameof(Entity.Client.Properties)}"
        }).ConfigureAwait(false);

        if (client is null)
        {
            return null;
        }

        var logoutUri = client.RedirectUris.FirstOrDefault(u => (u.Kind & Entity.UriKinds.PostLogout) == Entity.UriKinds.PostLogout)?.Uri;
        var acsUri = client.RedirectUris.FirstOrDefault(u => (u.Kind & Entity.UriKinds.Redirect) == Entity.UriKinds.Redirect)?.Uri;
        var signatureCertificateList = client.ClientSecrets
            .Where(s => (s.Expiration == null || s.Expiration > DateTime.UtcNow) &&
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

        var rp = client.RelyingParty;
        var encriptionCertificate = rp?.EncryptionCertificate is not null ?
            X509CertificateLoader.LoadCertificate(rp.EncryptionCertificate) : null;

        var relyingParty = new RelyingParty
        {
            Issuer = issuer,
            UseAcsArtifact = client.Properties.Any(p => p.Key == nameof(RelyingParty.UseAcsArtifact) && p.Value.ToUpperInvariant() == "TRUE"),
            AcsDestination = acsUri is not null ? new Uri(acsUri) : null,
            SingleLogoutDestination = logoutUri is not null ? new Uri(logoutUri) : null,
            SignatureValidationCertificate = signatureCertificateList,
            EncryptionCertificate = encriptionCertificate,
            SignatureAlgorithm = rp?.SignatureAlgorithm,
            SamlNameIdentifierFormat = rp?.SamlNameIdentifierFormat is not null ? new Uri(rp.SamlNameIdentifierFormat) : NameIdentifierFormats.Persistent,
            TokenType = rp?.TokenType,
        };

        if (rp?.ClaimMappings is not null)
        {
            relyingParty.ClaimMapping = rp.ClaimMappings.ToDictionary(m => m.FromClaimType, m => m.ToClaimType);
        }

        var metadata = client.RedirectUris.FirstOrDefault(u => u.Kind == Entity.UriKinds.Saml2Metadata);
        if (metadata != null)
        {
            return await LoadRelyingPartyFromMetadataAsync(relyingParty, metadata.Uri).ConfigureAwait(false);
        }
        return relyingParty;
    }

    private static X509Certificate2 GetCertificateFromBase64(string value)
    => X509CertificateLoader.LoadCertificate(Convert.FromBase64String(value));

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
        var spSsoDescriptor = entityDescriptor.SPSsoDescriptor;
        relyingParty.SamlNameIdentifierFormat = spSsoDescriptor.NameIDFormats?.FirstOrDefault() ?? NameIdentifierFormats.Persistent;
        relyingParty.AcsDestination = spSsoDescriptor.AssertionConsumerServices.Where(a => a.IsDefault).OrderBy(a => a.Index).First().Location;
        var singleLogoutService = spSsoDescriptor.SingleLogoutServices.First();
        relyingParty.SingleLogoutDestination = singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
        relyingParty.SignatureValidationCertificate = spSsoDescriptor.SigningCertificates;
        return relyingParty;
    }
}
