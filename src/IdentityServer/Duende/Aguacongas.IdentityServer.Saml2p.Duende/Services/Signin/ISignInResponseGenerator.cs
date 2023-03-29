using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
public interface ISignInResponseGenerator
{
    Task<IActionResult> GenerateLoginResponseAsync(SignInValidationResult result, Saml2StatusCodes status);
}
