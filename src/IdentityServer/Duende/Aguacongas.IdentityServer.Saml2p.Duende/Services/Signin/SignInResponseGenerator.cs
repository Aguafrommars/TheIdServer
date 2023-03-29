using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Services;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Security.Claims;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
public class SignInResponseGenerator : ISignInResponseGenerator
{
    private readonly IAdminStore<Entity.Saml2pArtifact> _store;
    private readonly IUserSession _userSession;
    private readonly IOptions<Saml2Configuration> _options;

    public SignInResponseGenerator(IAdminStore<Entity.Saml2pArtifact> store,
        IUserSession userSession,
        IOptions<Saml2Configuration> options)
    {
        _store = store;
        _userSession = userSession;
        _options = options;
    }

    public Task<IActionResult> GenerateLoginResponseAsync(SignInValidationResult result, Saml2StatusCodes status)
    => LoginResponse(result.Saml2Request?.Id, status, result.Saml2RedirectBinding?.RelayState, result.RelyingParty, Guid.NewGuid().ToString(), result.User, result.Client?.ClientId);

    private Task<IActionResult> LoginResponse(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty? relyingParty, string? sessionIndex = null, ClaimsPrincipal? user= null, string? clientId = null)
    {
        if (relyingParty?.UseAcsArtifact == true)
        {
            return LoginArtifactResponseAsync(inResponseTo, status, relayState, relyingParty, sessionIndex, user, clientId);
        }
        else
        {
            return Task.FromResult(LoginPostResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, user));
        }
    }

    private IActionResult LoginPostResponse(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty? relyingParty, string? sessionIndex = null, ClaimsPrincipal? user = null)
    {
        var responseBinding = new Saml2PostBinding
        {
            RelayState = relayState
        };

        var saml2AuthnResponse = new Saml2AuthnResponse(GetRelyingPartySaml2Configuration(relyingParty))
        {
            InResponseTo = inResponseTo,
            Status = status,
            Destination = relyingParty?.AcsDestination,
        };

        if (status == Saml2StatusCodes.Success && user?.Claims != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            var claimsIdentity = new ClaimsIdentity(user?.Claims);
            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
            saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

            saml2AuthnResponse.CreateSecurityToken(relyingParty?.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
        }

        return responseBinding.Bind(saml2AuthnResponse).ToActionResult();
    }

    private async Task<IActionResult> LoginArtifactResponseAsync(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty relyingParty, string? sessionIndex = null, ClaimsPrincipal? user = null, string? clientId = null)
    {
        var responseBinding = new Saml2ArtifactBinding
        {
            RelayState = relayState
        };

        var saml2ArtifactResolve = new Saml2ArtifactResolve(GetRelyingPartySaml2Configuration(relyingParty))
        {
            Destination = relyingParty.AcsDestination
        };
        responseBinding.Bind(saml2ArtifactResolve);

        var saml2AuthnResponse = new Saml2AuthnResponse(GetRelyingPartySaml2Configuration(relyingParty))
        {
            InResponseTo = inResponseTo,
            Status = status
        };

        if (status == Saml2StatusCodes.Success && user?.Claims != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            var claimsIdentity = new ClaimsIdentity(user?.Claims);
            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
            saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

            saml2AuthnResponse.CreateSecurityToken(relyingParty.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
        }

        var xml = saml2AuthnResponse.ToXml();
        await _store.CreateAsync(new Entity.Saml2pArtifact
        {
            Id = Guid.NewGuid().ToString(),
            Artifact = saml2ArtifactResolve.Artifact,
            Xml = xml.OuterXml,
            ClientId = clientId,
            SessionId = await _userSession.GetSessionIdAsync().ConfigureAwait(false),
            UserId = user?.FindFirstValue("sub"),
            CreatedAt = DateTime.UtcNow,
        }).ConfigureAwait(false);

        return responseBinding.ToActionResult();
    }

    private Saml2Configuration GetRelyingPartySaml2Configuration(RelyingParty? relyingParty)
    {
        var config = _options.Value;

        var rpConfig = new Saml2Configuration()
        {
            Issuer = config.Issuer,
            SingleSignOnDestination = config.SingleSignOnDestination,
            SingleLogoutDestination = config.SingleLogoutDestination,
            ArtifactResolutionService = config.ArtifactResolutionService,
            SigningCertificate = config.SigningCertificate,
            SignatureAlgorithm = config.SignatureAlgorithm,
            CertificateValidationMode = config.CertificateValidationMode,
            RevocationMode = config.RevocationMode
        };

        rpConfig.AllowedAudienceUris.AddRange(config.AllowedAudienceUris);

        if (relyingParty is not null)
        {
            rpConfig.SignatureValidationCertificates.Add(relyingParty.SignatureValidationCertificate);
            rpConfig.EncryptionCertificate = relyingParty.EncryptionCertificate;
        }

        return rpConfig;
    }

}
