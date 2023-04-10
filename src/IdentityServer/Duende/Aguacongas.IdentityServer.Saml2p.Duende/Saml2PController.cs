using Aguacongas.IdentityServer.Saml2p.Duende.Services;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.IdentityServer.Saml2p.Duende;

/// <summary>
/// Saml2P controller
/// </summary>
[Route("[controller]")]
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This is a controller")]
public class Saml2PController : Controller
{
    /// <summary>
    /// Metadata endpoint
    /// </summary>
    /// <param name="metadataResponseGenerator"></param>
    /// <returns></returns>
    [HttpGet("metadata")]
    public Task<IActionResult> Metadata([FromServices] IMetadataResponseGenerator metadataResponseGenerator)
    => metadataResponseGenerator.GenerateMetadataResponseAsync();

    /// <summary>
    /// Login endpoint
    /// </summary>
    /// <param name="saml2PService"></param>
    /// <returns></returns>
    [Route("login")]
    public Task<IActionResult> Login([FromServices] ISaml2PService saml2PService)
    => saml2PService.LoginAsync(Request, Url);

    /// <summary>
    /// Artifact endpoint
    /// </summary>
    /// <param name="saml2PService"></param>
    /// <returns></returns>
    [Route("artifact")]
    public Task<IActionResult> Artifact([FromServices] ISaml2PService saml2PService)
    => saml2PService.ArtifactAsync(Request);

    /// <summary>
    /// Logout endpoint
    /// </summary>
    /// <param name="saml2PService"></param>
    /// <returns></returns>
    [HttpPost("logout")]
    public Task<IActionResult> Logout([FromServices] ISaml2PService saml2PService)
    => saml2PService.LogoutAsync(Request);
}
    