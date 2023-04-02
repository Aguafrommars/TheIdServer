using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Metatdata;

public interface IMetadataResponseGenerator
{
    Task<IActionResult> GenerateMetadataResponseAsync();
}