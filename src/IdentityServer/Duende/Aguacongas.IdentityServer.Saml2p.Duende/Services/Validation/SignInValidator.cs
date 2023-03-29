using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Duende.IdentityServer;
using Duende.IdentityServer.Stores;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
public class SignInValidator : ISignInValidator
{
    private readonly IClientStore _clientStore;
    private readonly IRelyingPartyStore _relyingPartyStore;
    private readonly IOptions<Saml2Configuration> _options;

    public SignInValidator(IClientStore clientStore,
        IRelyingPartyStore relyingPartyStore,
        IOptions<Saml2Configuration> options)        
    {
        _clientStore = clientStore;
        _relyingPartyStore = relyingPartyStore;
        _options = options;
    }

    public async Task<SignInValidationResult<Saml2SoapEnvelope>> ValidateArtifactRequestAsync(HttpRequest request)
    {
        var soapEnvelope = new Saml2SoapEnvelope();
        var httpRequest = await request.ToGenericHttpRequestAsync(readBodyAsString: true).ConfigureAwait(false);
        var settings = _options.Value;
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
            RelyingParty = rp,
        };
    }

    public async Task<SignInValidationResult<Saml2RedirectBinding>> ValidateAsync(HttpRequest request, ClaimsPrincipal user)
    {
        var genericRequest = request.ToGenericHttpRequest();
        var settings = _options.Value;
        var saml2AuthnRequest = new Saml2AuthnRequest(settings);
        var requestBinding = new Saml2RedirectBinding();
        requestBinding.ReadSamlRequest(genericRequest, saml2AuthnRequest);

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

}
