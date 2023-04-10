using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Duende.IdentityServer;
using Duende.IdentityServer.Stores;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;

/// <summary>
/// Signing requests validator
/// </summary>
public class SignInValidator : ISignInValidator
{
    private readonly IClientStore _clientStore;
    private readonly IRelyingPartyStore _relyingPartyStore;
    private readonly ISaml2ConfigurationService _configurationService;

    /// <summary>
    /// Initialize a new instance of <see cref="SignInValidator"/>
    /// </summary>
    /// <param name="clientStore"></param>
    /// <param name="relyingPartyStore"></param>
    /// <param name="configurationService"></param>
    public SignInValidator(IClientStore clientStore,
        IRelyingPartyStore relyingPartyStore,
        ISaml2ConfigurationService configurationService)
    {
        _clientStore = clientStore;
        _relyingPartyStore = relyingPartyStore;
        _configurationService = configurationService;
    }

    /// <summary>
    /// Validates artifact request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SignInValidationResult<Saml2SoapEnvelope>> ValidateArtifactRequestAsync(HttpRequest request)
    {
        var soapEnvelope = new Saml2SoapEnvelope();
        var httpRequest = await request.ToGenericHttpRequestAsync(readBodyAsString: true).ConfigureAwait(false);
        var settings = await _configurationService.GetConfigurationAsync().ConfigureAwait(false);
        var saml2ArtifactResolve = new Saml2ArtifactResolve(settings);

        var samlRequest = soapEnvelope.ReadSamlRequest(httpRequest, saml2ArtifactResolve);
        var issuer = samlRequest.Issuer;

        // check client
        var client = await _clientStore.FindEnabledClientByIdAsync(issuer);

        if (client == null)
        {
            return new SignInValidationResult<Saml2SoapEnvelope>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} client not found."
            };
        }
        if (client.ProtocolType != IdentityServerConstants.ProtocolTypes.Saml2p)
        {
            return new SignInValidationResult<Saml2SoapEnvelope>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} client is not a saml2p client."
            };
        }

        var rp = await _relyingPartyStore.FindRelyingPartyAsync(issuer).ConfigureAwait(false);
        if (rp == null)
        {
            return new SignInValidationResult<Saml2SoapEnvelope>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} relying party not found."
            };
        }

        return new SignInValidationResult<Saml2SoapEnvelope>
        {
            Saml2Request = samlRequest,
            Saml2Binding = soapEnvelope,
            GerericRequest = httpRequest,
            Client = client,
            RelyingParty = await _relyingPartyStore.FindRelyingPartyAsync(issuer).ConfigureAwait(false),
        };
    }

    /// <summary>
    /// Validates login request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<SignInValidationResult<Saml2RedirectBinding>> ValidateLoginAsync(HttpRequest request, ClaimsPrincipal user)
    {
        var genericRequest = request.ToGenericHttpRequest();
        var settings = await _configurationService.GetConfigurationAsync().ConfigureAwait(false);
        var saml2AuthnRequest = new Saml2AuthnRequest(settings);
        var requestBinding = new Saml2RedirectBinding();

        try
        {
            requestBinding.ReadSamlRequest(genericRequest, saml2AuthnRequest);
        }
        catch (InvalidSaml2BindingException ex)
        {
            return new SignInValidationResult<Saml2RedirectBinding>
            {
                Error = nameof(InvalidSaml2BindingException),
                ErrorMessage = ex.Message
            };
        }

        var issuer = saml2AuthnRequest.Issuer;

        // check client
        var client = await _clientStore.FindEnabledClientByIdAsync(issuer);

        if (client == null)
        {
            return new SignInValidationResult<Saml2RedirectBinding>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} client not found."
            };
        }
        if (client.ProtocolType != IdentityServerConstants.ProtocolTypes.Saml2p)
        {
            return new SignInValidationResult<Saml2RedirectBinding>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} client is not a saml2p client."
            };
        }

        var rp = await _relyingPartyStore.FindRelyingPartyAsync(issuer).ConfigureAwait(false);
        if (rp == null)
        {
            return new SignInValidationResult<Saml2RedirectBinding>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} relying party not found."
            };
        }

        return new SignInValidationResult<Saml2RedirectBinding>
        {
            Saml2Request = saml2AuthnRequest,
            Saml2Binding = requestBinding,
            GerericRequest = genericRequest,
            Client = client,
            RelyingParty = rp,
            User = user,
            SignInRequired = user?.Identity?.IsAuthenticated != true
        };
    }

    /// <summary>
    /// Validates logout request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<SignInValidationResult<Saml2PostBinding>> ValidateLogoutAsync(HttpRequest request)
    {
        var genericRequest = request.ToGenericHttpRequest();
        var settings = await _configurationService.GetConfigurationAsync().ConfigureAwait(false);
        var saml2LogoutRequest = new Saml2LogoutRequest(settings);
        var requestBinding = new Saml2PostBinding();
        
        var samlRequest = requestBinding.ReadSamlRequest(genericRequest, saml2LogoutRequest);

        var issuer = samlRequest.Issuer;

        // check client
        var client = await _clientStore.FindEnabledClientByIdAsync(issuer);

        if (client == null)
        {
            return new SignInValidationResult<Saml2PostBinding>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} client not found."
            };
        }
        if (client.ProtocolType != IdentityServerConstants.ProtocolTypes.Saml2p)
        {
            return new SignInValidationResult<Saml2PostBinding>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} client is not a saml2p client."
            };
        }

        var rp = await _relyingPartyStore.FindRelyingPartyAsync(issuer).ConfigureAwait(false);
        if (rp == null)
        {
            return new SignInValidationResult<Saml2PostBinding>
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{issuer} relying party not found."
            };
        }

        return new SignInValidationResult<Saml2PostBinding>
        {
            Saml2Request = samlRequest,
            Saml2Binding = requestBinding,
            GerericRequest = genericRequest,
            Client = client,
            RelyingParty = rp
        };
    }
}
