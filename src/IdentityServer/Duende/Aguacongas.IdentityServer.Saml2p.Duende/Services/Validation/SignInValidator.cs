using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Duende.IdentityServer;
using Duende.IdentityServer.Stores;
using ITfoxtec.Identity.Saml2;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
public class SignInValidator : ISignInValidator
{
    private readonly IClientStore _clientStore;
    private readonly IRelyingPartyStore _relyingPartyStore;

    public SignInValidator(IClientStore clientStore,
        IRelyingPartyStore relyingPartyStore)
    {
        _clientStore = clientStore;
        _relyingPartyStore = relyingPartyStore;
    }

    public async Task<SignInValidationResult> ValidateAsync(Saml2Request request, ClaimsPrincipal user)
    {
        var result = new SignInValidationResult
        {
            Saml2Request = request
        };

        // check client
        var client = await _clientStore.FindEnabledClientByIdAsync(request.Issuer);

        if (client == null)
        {
            return new SignInValidationResult
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{request.Issuer} client not found."
            };
        }
        if (client.ProtocolType != IdentityServerConstants.ProtocolTypes.Saml2p)
        {
            return new SignInValidationResult
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{request.Issuer} client is not a saml2p client."
            };
        }

        var rp = await _relyingPartyStore.FindRelyingPartyAsync(request.Issuer).ConfigureAwait(false);
        if (rp == null)
        {
            return new SignInValidationResult
            {
                Error = "invalid_relying_party",
                ErrorMessage = $"{request.Issuer} relying party not found."
            };
        }

        result.Client = client;
        result.RelyingParty = rp;

        if (user?.Identity?.IsAuthenticated != true)
        {
            result.SignInRequired = true;
        }

        result.User = user;

        return result;
    }
}
