using Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Duende.IdentityServer.Services;
using IdentityModel;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
public class SignInResponseGenerator : ISignInResponseGenerator
{
    private readonly IArtifactStore _store;
    private readonly IUserSession _userSession;
    private readonly ISaml2ConfigurationService _configurationService;

    public SignInResponseGenerator(IArtifactStore store,
        IUserSession userSession,
        ISaml2ConfigurationService configurationService)
    {
        _store = store;
        _userSession = userSession;
        _configurationService = configurationService;
    }

    public async Task<IActionResult> GenerateArtifactResponseAsync(SignInValidationResult<Saml2SoapEnvelope> result)
    {
        var relyingParty = result.RelyingParty;
        var saml2ArtifactResolve = new Saml2ArtifactResolve(await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false));
        
        var soapEnvelope = result.Saml2Binding ?? throw new InvalidOperationException("SoapEnvelope cannot be null.");
        soapEnvelope.Unbind(result.GerericRequest, saml2ArtifactResolve);

        var artifact = await _store.RemoveAsync(saml2ArtifactResolve.Artifact).ConfigureAwait(false);

        var saml2AuthnResponse = new InternalSaml2AuthnResponse(await GetRelyingPartySaml2ConfigurationAsync(result.RelyingParty).ConfigureAwait(false),
            artifact.Xml);
        var saml2ArtifactResponse = new Saml2ArtifactResponse(await _configurationService.GetConfigurationAsync().ConfigureAwait(false),
            saml2AuthnResponse)
        {
            InResponseTo = saml2ArtifactResolve.Id
        };
        soapEnvelope?.Bind(saml2ArtifactResponse);

        return soapEnvelope.ToActionResult();
    }

    public Task<IActionResult> GenerateLoginResponseAsync(SignInValidationResult<Saml2RedirectBinding> result, Saml2StatusCodes status)
    => LoginResponse(result.Saml2Request?.Id, status, result.Saml2Binding?.RelayState, result.RelyingParty, Guid.NewGuid().ToString(), result.User, result.Client?.ClientId);

    public async Task<IActionResult> GenerateLogoutResponseAsync(SignInValidationResult<Saml2PostBinding> signinResult, Saml2StatusCodes status)
    {
        var responseBinding = new Saml2PostBinding();
        responseBinding.RelayState = signinResult.Saml2Binding?.RelayState;
        var relyingParty = signinResult.RelyingParty;
        var samlLogoutRequest = signinResult.Saml2Request;

        var saml2LogoutResponse = new Saml2LogoutResponse(await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false))
        {
            InResponseTo = samlLogoutRequest?.Id,
            Status = status,
            Destination = relyingParty?.SingleLogoutDestination,
            SessionIndex = samlLogoutRequest?.SessionIndex
        };

        await _userSession.RemoveSessionIdCookieAsync().ConfigureAwait(false);

        return responseBinding.Bind(saml2LogoutResponse).ToActionResult();
    }

    private Task<IActionResult> LoginResponse(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty? relyingParty, string? sessionIndex = null, ClaimsPrincipal? user= null, string? clientId = null)
    {
        if (relyingParty?.UseAcsArtifact == true)
        {
            return LoginArtifactResponseAsync(inResponseTo, status, relayState, relyingParty, sessionIndex, user, clientId);
        }
        else
        {
            return LoginPostResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, user);
        }
    }

    private async Task<IActionResult> LoginPostResponse(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty? relyingParty, string? sessionIndex = null, ClaimsPrincipal? user = null)
    {
        var responseBinding = new Saml2PostBinding
        {
            RelayState = relayState
        };

        var saml2AuthnResponse = new Saml2AuthnResponse(await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false))
        {
            InResponseTo = inResponseTo,
            Status = status,
            Destination = relyingParty?.AcsDestination,
        };

        if (status == Saml2StatusCodes.Success && user?.Claims != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            var claimsIdentity = new ClaimsIdentity(user?.Claims);
            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == JwtClaimTypes.Name).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
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

        var saml2ArtifactResolve = new Saml2ArtifactResolve(await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false))
        {
            Destination = relyingParty.AcsDestination
        };
        responseBinding.Bind(saml2ArtifactResolve);

        var saml2AuthnResponse = new Saml2AuthnResponse(await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false))
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
        await _store.StoreAsync(new IdentityServer.Store.Entity.Saml2pArtifact
        {
            Id = saml2ArtifactResolve.Artifact,
            ClientId = clientId,
            CreatedAt = DateTime.UtcNow,
            SessionId = await _userSession.GetSessionIdAsync().ConfigureAwait(false),
            UserId = user?.FindFirstValue("sub"),
            Xml = xml.OuterXml
        }).ConfigureAwait(false);

        return responseBinding.ToActionResult();
    }

    private async Task<Saml2Configuration> GetRelyingPartySaml2ConfigurationAsync(RelyingParty? relyingParty)
    {
        var config = await _configurationService.GetConfigurationAsync().ConfigureAwait(false);

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
            rpConfig.SignatureValidationCertificates.AddRange(relyingParty.SignatureValidationCertificate);
            rpConfig.EncryptionCertificate = relyingParty.EncryptionCertificate;
        }

        return rpConfig;
    }

    class InternalSaml2AuthnResponse : Saml2AuthnResponse
    {
        public InternalSaml2AuthnResponse(Saml2Configuration config, string xml) : base(config)
        {
            Read(xml, true);
        }
    }
}
