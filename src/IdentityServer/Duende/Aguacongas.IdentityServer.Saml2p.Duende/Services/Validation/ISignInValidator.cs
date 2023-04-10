using ITfoxtec.Identity.Saml2;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;

/// <summary>
/// Signing requests validator interface
/// </summary>
public interface ISignInValidator
{
    /// <summary>
    /// Validates artifact request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<SignInValidationResult<Saml2SoapEnvelope>> ValidateArtifactRequestAsync(HttpRequest request);
    
    /// <summary>
    /// Validates login request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<SignInValidationResult<Saml2RedirectBinding>> ValidateLoginAsync(HttpRequest request, ClaimsPrincipal user);

    /// <summary>
    /// Validates logout request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<SignInValidationResult<Saml2PostBinding>> ValidateLogoutAsync(HttpRequest request);
}
