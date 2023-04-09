using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Metadata;

/// <summary>
/// Metadata response generator interface
/// </summary>
public interface IMetadataResponseGenerator
{
    /// <summary>
    /// Generates the metadata response
    /// </summary>
    /// <returns></returns>
    Task<IActionResult> GenerateMetadataResponseAsync();
}