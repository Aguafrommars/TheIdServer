using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
public interface ISignInValidator
{
    Task<SignInValidationResult> ValidateAsync(HttpRequest request, ClaimsPrincipal user);
}
