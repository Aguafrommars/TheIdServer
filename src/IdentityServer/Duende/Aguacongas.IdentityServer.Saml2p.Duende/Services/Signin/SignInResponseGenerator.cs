using Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Security.Claims;
using ClaimProperties = Microsoft.IdentityModel.Tokens.Saml.ClaimProperties;
using ISValidation = Duende.IdentityServer.Validation;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;

/// <summary>
/// Signing  response generator
/// </summary>
public class SignInResponseGenerator : ISignInResponseGenerator
{
    private readonly IArtifactStore _store;
    private readonly IUserSession _userSession;
    private readonly ISaml2ConfigurationService _configurationService;
    private readonly IResourceStore _resources;
    private readonly IProfileService _profile;
    private readonly IOptions<Saml2POptions> _options;
    private readonly ILogger<SignInResponseGenerator> _logger;

    /// <summary>
    /// Initialize a new instance of <see cref="SignInResponseGenerator"/>
    /// </summary>
    /// <param name="store"></param>
    /// <param name="userSession"></param>
    /// <param name="configurationService"></param>
    /// <param name="resource"></param>
    /// <param name="profile"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public SignInResponseGenerator(IArtifactStore store,
        IUserSession userSession,
        ISaml2ConfigurationService configurationService,
        IResourceStore resource,
        IProfileService profile,
        IOptions<Saml2POptions> options,
        ILogger<SignInResponseGenerator> logger)
    {
        _store = store;
        _userSession = userSession;
        _configurationService = configurationService;
        _resources = resource;
        _profile = profile;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Generates artifact response
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IActionResult> GenerateArtifactResponseAsync(SignInValidationResult<Saml2SoapEnvelope> result)
    {
        var relyingParty = result.RelyingParty;
        var relyingPartyConfig = await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false);
        var saml2ArtifactResolve = new Saml2ArtifactResolve(relyingPartyConfig);
        
        var soapEnvelope = result.Saml2Binding ?? throw new InvalidOperationException("SoapEnvelope cannot be null.");
        soapEnvelope.Unbind(result.GerericRequest, saml2ArtifactResolve);

        var artifact = await _store.RemoveAsync(saml2ArtifactResolve.Artifact).ConfigureAwait(false);

        relyingPartyConfig.AllowedAudienceUris.Add(relyingParty?.Issuer);
        var saml2AuthnResponse = new InternalSaml2AuthnResponse(relyingPartyConfig, artifact.Data);
        var saml2ArtifactResponse = new Saml2ArtifactResponse(await _configurationService.GetConfigurationAsync().ConfigureAwait(false),
            saml2AuthnResponse)
        {
            InResponseTo = saml2ArtifactResolve.Id
        };
        soapEnvelope.Bind(saml2ArtifactResponse);

        if (result.Client?.ClientId is not null)
        {
            await _userSession.AddClientIdAsync(result.Client.ClientId).ConfigureAwait(false);
        }        

        return soapEnvelope.ToActionResult();
    }

    /// <summary>
    /// Generates login response
    /// </summary>
    /// <param name="result"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<IActionResult> GenerateLoginResponseAsync(SignInValidationResult<Saml2RedirectBinding> result, Saml2StatusCodes status)
    {
        var claimsIdentity = await CreateSubjectAsync(result).ConfigureAwait(false);
        var relyingParty = result.RelyingParty;
        var inResponseTo = result.Saml2Request?.Id;
        var relayState = result.Saml2Binding?.RelayState;
        var sessionIndex = Guid.NewGuid().ToString();
        var clientId = result.Client?.ClientId;

        if (relyingParty?.UseAcsArtifact == true)
        {
            var subjectId = result.User?.FindFirst(JwtClaimTypes.Subject);
            if (subjectId is not null)
            {
                claimsIdentity.AddClaim(subjectId);
            }

            return await LoginArtifactResponseAsync(inResponseTo, status, relayState, relyingParty, sessionIndex, claimsIdentity, clientId)
                .ConfigureAwait(false);
        }
        else
        {
            return await LoginPostResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, claimsIdentity, clientId)
                .ConfigureAwait(false);
        }
    }
    /// <summary>
    /// Generates logout response
    /// </summary>
    /// <param name="result"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<IActionResult> GenerateLogoutResponseAsync(SignInValidationResult<Saml2PostBinding> result, Saml2StatusCodes status)
    {
        var responseBinding = new Saml2PostBinding
        {
            RelayState = result.Saml2Binding?.RelayState
        };
        var relyingParty = result.RelyingParty;
        var samlLogoutRequest = result.Saml2Request;

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

    private async Task<IActionResult> LoginPostResponse(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty? relyingParty, string? sessionIndex = null, ClaimsIdentity? claimsIdentity = null, string? clientId = null)
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

        if (status == Saml2StatusCodes.Success && claimsIdentity != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims
                .Where(c => c.Type == JwtClaimTypes.Name)
                .Select(c => c.Value)
                .Single(), relyingParty?.SamlNameIdentifierFormat);
            saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

            var settings = _options.Value;
            saml2AuthnResponse.CreateSecurityToken(relyingParty?.Issuer, 
                subjectConfirmationLifetime: settings.SubjectConfirmationLifetime, 
                issuedTokenLifetime: settings.IssuedTokenLifetime);
        }

        if (status == Saml2StatusCodes.Success && clientId is not null)
        {
            await _userSession.AddClientIdAsync(clientId).ConfigureAwait(false);
        }

        return responseBinding.Bind(saml2AuthnResponse).ToActionResult();
    }

    private async Task<IActionResult> LoginArtifactResponseAsync(Saml2Id? inResponseTo, Saml2StatusCodes status, string? relayState, RelyingParty relyingParty, string? sessionIndex = null, ClaimsIdentity? claimsIdentity = null, string? clientId = null)
    {
        var responseBinding = new Saml2ArtifactBinding
        {
            RelayState = relayState
        };

        var config = await GetRelyingPartySaml2ConfigurationAsync(relyingParty).ConfigureAwait(false);
        var saml2ArtifactResolve = new Saml2ArtifactResolve(config)
        {
            Destination = relyingParty.AcsDestination
        };
        responseBinding.Bind(saml2ArtifactResolve);

        var saml2AuthnResponse = new Saml2AuthnResponse(config)
        {
            InResponseTo = inResponseTo,
            Status = status
        };

        var settings = _options.Value;
        var userId = claimsIdentity?.GetSubjectId();
        if (status == Saml2StatusCodes.Success && claimsIdentity != null)
        {
            saml2AuthnResponse.SessionIndex = sessionIndex;

            var nameIdFormat = relyingParty.SamlNameIdentifierFormat.ToString();
            saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims
                .Where(c => c.Type == nameIdFormat)
                .Select(c => c.Value)
                .Single(), relyingParty.SamlNameIdentifierFormat);
            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(JwtClaimTypes.Subject));
            saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

            saml2AuthnResponse.CreateSecurityToken(relyingParty.Issuer,
                subjectConfirmationLifetime: settings.SubjectConfirmationLifetime, 
                issuedTokenLifetime: settings.IssuedTokenLifetime);
        }

        var xml = saml2AuthnResponse.ToXml();

        await _store.StoreAsync(new IdentityServer.Store.Entity.Saml2PArtifact
        {
            Id = saml2ArtifactResolve.Artifact,
            ClientId = clientId,
            CreatedAt = DateTime.UtcNow,
            SessionId = await _userSession.GetSessionIdAsync().ConfigureAwait(false),
            UserId = userId,
            Data = xml.OuterXml,
            Expiration = DateTime.UtcNow.AddMinutes(settings.SubjectConfirmationLifetime),
        }).ConfigureAwait(false);

        return responseBinding.ToActionResult();
    }

    /// <summary>
    /// Get relying party saml2 configuration
    /// </summary>
    /// <param name="relyingParty"></param>
    /// <returns></returns>
    public async Task<Saml2Configuration> GetRelyingPartySaml2ConfigurationAsync(RelyingParty? relyingParty)
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

    /// <summary>
    /// Creates the subject asynchronous.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    protected async Task<ClaimsIdentity> CreateSubjectAsync(SignInValidationResult<Saml2RedirectBinding> result)
    {
        var user = result.User;
        if (user is null)
        {
            return new ClaimsIdentity();
        }

        var requestedClaimTypes = new List<string>();

        var resources = await _resources.FindEnabledIdentityResourcesByScopeAsync(result.Client?.AllowedScopes).ConfigureAwait(false);
        foreach (var resource in resources)
        {
            foreach (var claim in resource.UserClaims)
            {
                requestedClaimTypes.Add(claim);
            }
        }

        var client = result.Client;
#pragma warning disable CS8601 // Possible null reference assignment.
        var ctx = new ProfileDataRequestContext
        {
            Subject = user,
            RequestedClaimTypes = requestedClaimTypes,
            Client = client,
            Caller = "SAML 2.0",
            RequestedResources = new ISValidation.ResourceValidationResult
            {
                Resources = new Resources
                {
                    IdentityResources = resources.ToList()
                }
            }
        };
#pragma warning restore CS8601 // Possible null reference assignment.

        await _profile.GetProfileDataAsync(ctx).ConfigureAwait(false);

        if (client is not null)
        {
            ctx.IssuedClaims.AddRange(client.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)));
        }
        // map outbound claims
        var relyParty = result.RelyingParty;
        var mapping = relyParty.ClaimMapping;

        
        var nameidFormat = relyParty.SamlNameIdentifierFormat?.ToString() ?? NameIdentifierFormats.Persistent.ToString();
        var nameid = new Claim(nameidFormat, user.GetDisplayName());

        var outboundClaims = new List<Claim> 
        { 
            nameid,
        };
        foreach (var claim in ctx.IssuedClaims)
        {
            if (mapping?.ContainsKey(claim.Type) == true)
            {
                AddMappedClaim(nameidFormat, outboundClaims, mapping, claim);
            }
            else if (Uri.TryCreate(claim.Type, UriKind.Absolute, out Uri? _) ||
                relyParty.TokenType != "urn:oasis:names:tc:SAML:1.0:assertion")
            {
                outboundClaims.Add(claim);
            }
            else
            {
                _logger.LogInformation("No explicit claim type mapping for {claimType} configured. Saml requires a URI claim type. Skipping.", claim.Type);
            }
        }

        return new ClaimsIdentity(outboundClaims, "theidserver");
    }

    private static void AddMappedClaim(string nameIdFormat, List<Claim> outboundClaims, IDictionary<string, string> mapping, Claim claim)
    {
        var outboundClaim = new Claim(mapping[claim.Type], claim.Value, claim.ValueType);
        if (outboundClaim.Type == ClaimTypes.NameIdentifier)
        {
            outboundClaim.Properties[ClaimProperties.SamlNameIdentifierFormat] = nameIdFormat;
            outboundClaims.RemoveAll(c => c.Type == ClaimTypes.NameIdentifier); //remove previesly added nameid claim
        }
        if (outboundClaim.Type == ClaimTypes.Name)
        {
            outboundClaims.RemoveAll(c => c.Type == ClaimTypes.Name); //remove previesly added name claim
        }

        outboundClaims.Add(outboundClaim);
    }
    class InternalSaml2AuthnResponse : Saml2AuthnResponse
    {
        public InternalSaml2AuthnResponse(Saml2Configuration config, string xml) : base(config)
        {
            Read(xml);
        }
    }
}
