using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Services;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services;
public class Saml2PService : ISaml2PService
{
    private readonly ISignInValidator _signInValidator;
    private readonly IUserSession _userSession;
    private readonly IOptions<Saml2Configuration> _saml2Options;
    private readonly IOptions<IdentityServerOptions> _identityServerOptions;
    private readonly ILogger<Saml2PService> _logger;

    public Saml2PService(ISignInValidator signInValidator,
        IUserSession userSession,
        IOptions<Saml2Configuration> saml2Options,
        IOptions<IdentityServerOptions> identityServerOptions,
        ILogger<Saml2PService> logger)
    {
        _signInValidator = signInValidator;
        _userSession = userSession;
        _saml2Options = saml2Options;
        _identityServerOptions = identityServerOptions;
        _logger = logger;
    }

    public Task<IActionResult> ArtifactAsync(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IActionResult> LoginAsync(HttpRequest request, IUrlHelper helper)
    {
        var genericRequest = request.ToGenericHttpRequest();
        var settings = _saml2Options.Value;
        var saml2AuthnRequest = new Saml2AuthnRequest(settings);
        var requestBinding = new Saml2RedirectBinding();
        requestBinding.ReadSamlRequest(genericRequest, saml2AuthnRequest);

        var user = await _userSession.GetUserAsync().ConfigureAwait(false);

        var signinResult = await _signInValidator.ValidateAsync(requestBinding.ReadSamlRequest(genericRequest, saml2AuthnRequest), user).ConfigureAwait(false);

        if (signinResult.Error is not null)
        {
            _logger.LogError(signinResult.ErrorMessage);
            return LoginResponse(saml2AuthnRequest.Id, Saml2StatusCodes.Requester, requestBinding.RelayState, signinResult.RelyingParty);
        }

        if (signinResult.SignInRequired)
        {
            var returnUrl = helper.Action(nameof(AuthController.Login));
            returnUrl = AddQueryString(returnUrl, request.QueryString.Value);

            var userInteraction = _identityServerOptions.Value.UserInteraction;
            var loginUrl = request.PathBase + userInteraction.LoginUrl;
            var url = AddQueryString(loginUrl, userInteraction.LoginReturnUrlParameter, returnUrl);

            return new RedirectResult(url);
        }

        try
        {
            requestBinding.Unbind(genericRequest, saml2AuthnRequest);

            return LoginResponse(saml2AuthnRequest.Id, 
                Saml2StatusCodes.Success, 
                requestBinding.RelayState, 
                signinResult.RelyingParty, 
                Guid.NewGuid().ToString(), 
                user.Claims);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, exc.Message);
            return LoginResponse(saml2AuthnRequest.Id, Saml2StatusCodes.Responder, requestBinding.RelayState, signinResult.RelyingParty);
        }
    }

    public Task<IActionResult> LogoutAsync(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    private IActionResult LoginResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, RelyingParty? relyingParty, string? sessionIndex = null, IEnumerable<Claim>? claims = null)
    {
        if (relyingParty?.UseAcsArtifact == true)
        {
            return LoginArtifactResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, claims);
        }
        else
        {
            return LoginPostResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, claims);
        }
    }

    private IActionResult LoginPostResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, RelyingParty? relyingParty, string? sessionIndex = null, IEnumerable<Claim>? claims = null)
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

        if (status == Saml2StatusCodes.Success && claims != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            var claimsIdentity = new ClaimsIdentity(claims);
            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
            saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

            saml2AuthnResponse.CreateSecurityToken(relyingParty?.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
        }

        return responseBinding.Bind(saml2AuthnResponse).ToActionResult();
    }

    private IActionResult LoginArtifactResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, RelyingParty relyingParty, string? sessionIndex = null, IEnumerable<Claim>? claims = null)
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

        if (status == Saml2StatusCodes.Success && claims != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            var claimsIdentity = new ClaimsIdentity(claims);
            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
            saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

            saml2AuthnResponse.CreateSecurityToken(relyingParty.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
        }

        return responseBinding.ToActionResult();
    }

    private Saml2Configuration GetRelyingPartySaml2Configuration(RelyingParty? relyingParty)
    {
        var config = _saml2Options.Value;

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

    private static string AddQueryString(string? url, string? query)
    {
        if (url?.Contains('?') == false)
        {
            if (query?.StartsWith("?") == false)
            {
                url += "?";
            }
        }
        else if (url?.EndsWith("&") == false)
        {
            url += "&";
        }

        return url + query;
    }

    private static string AddQueryString(string url, string name, string value)
    {
        return AddQueryString(url, $"{name}={UrlEncoder.Default.Encode(value)}");
    }
}
