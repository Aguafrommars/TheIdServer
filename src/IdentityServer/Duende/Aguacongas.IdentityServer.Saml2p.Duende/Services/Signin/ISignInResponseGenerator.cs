using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;

/// <summary>
/// Signing response generator interface
/// </summary>
public interface ISignInResponseGenerator
{
    /// <summary>
    /// Generates artifact response
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    Task<IActionResult> GenerateArtifactResponseAsync(SignInValidationResult<Saml2SoapEnvelope> result);
    
    /// <summary>
    /// Generates login response
    /// </summary>
    /// <param name="result"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<IActionResult> GenerateLoginResponseAsync(SignInValidationResult<Saml2RedirectBinding> result, Saml2StatusCodes status);
    
    /// <summary>
    /// Generates logout response
    /// </summary>
    /// <param name="result"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<IActionResult> GenerateLogoutResponseAsync(SignInValidationResult<Saml2PostBinding> result, Saml2StatusCodes status);
}
