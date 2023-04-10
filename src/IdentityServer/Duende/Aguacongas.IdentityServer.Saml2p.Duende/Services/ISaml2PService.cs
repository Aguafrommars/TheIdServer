using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services;

/// <summary>
/// Saml2P service interface
/// </summary>
public interface ISaml2PService
{
    /// <summary>
    /// Handles login request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="helper"></param>
    /// <returns></returns>
    Task<IActionResult> LoginAsync(HttpRequest request, IUrlHelper helper);

    /// <summary>
    /// Handles logout request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IActionResult> LogoutAsync(HttpRequest request);

    /// <summary>
    /// Handles artifact request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IActionResult> ArtifactAsync(HttpRequest request);
}
