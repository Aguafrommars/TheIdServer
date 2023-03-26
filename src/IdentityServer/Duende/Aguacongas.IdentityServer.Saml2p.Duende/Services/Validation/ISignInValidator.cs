using ITfoxtec.Identity.Saml2;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
public interface ISignInValidator
{
    Task<SignInValidationResult> ValidateAsync(Saml2Request request, ClaimsPrincipal user);
}
