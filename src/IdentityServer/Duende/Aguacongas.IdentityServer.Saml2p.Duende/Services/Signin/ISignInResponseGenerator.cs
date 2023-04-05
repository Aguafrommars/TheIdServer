using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
public interface ISignInResponseGenerator
{
    Task<IActionResult> GenerateArtifactResponseAsync(SignInValidationResult<Saml2SoapEnvelope> result);
    Task<IActionResult> GenerateLoginResponseAsync(SignInValidationResult<Saml2RedirectBinding> result, Saml2StatusCodes status);
    Task<IActionResult> GenerateLogoutResponseAsync(SignInValidationResult<Saml2PostBinding> signinResult, Saml2StatusCodes status);
}
