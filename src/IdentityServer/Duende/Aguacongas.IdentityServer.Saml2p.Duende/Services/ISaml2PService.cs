using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services;
public interface ISaml2PService
{
    Task<IActionResult> LoginAsync(HttpRequest request, IUrlHelper helper);

    Task<IActionResult> LogoutAsync(HttpRequest request);

    Task<IActionResult> ArtifactAsync(HttpRequest request);
}
