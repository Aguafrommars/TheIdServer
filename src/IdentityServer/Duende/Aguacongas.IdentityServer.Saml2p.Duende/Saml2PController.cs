using Aguacongas.IdentityServer.Saml2p.Duende.Services;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Metatdata;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.IdentityServer.Saml2p.Duende
{
    [Route("[controller]")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This is a controller")]
    public class Saml2PController : Controller
    {
        [HttpGet("metadata")]
        public Task<IActionResult> Metadata([FromServices] IMetadataResponseGenerator metadataResponseGenerator)
        => metadataResponseGenerator.GenerateMetadataResponseAsync();

        [Route("login")]
        public Task<IActionResult> Login([FromServices] ISaml2PService saml2PService)
        => saml2PService.LoginAsync(Request, Url);

        [Route("artifact")]
        public Task<IActionResult> Artifact([FromServices] ISaml2PService saml2PService)
        => saml2PService.ArtifactAsync(Request);

        [HttpPost("logout")]
        public Task<IActionResult> Logout([FromServices] ISaml2PService saml2PService)
        => saml2PService.LogoutAsync(Request);
    }
}        