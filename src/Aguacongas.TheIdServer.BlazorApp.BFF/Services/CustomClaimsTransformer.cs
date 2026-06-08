using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Services;

public class CustomClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
        {
            return Task.FromResult(principal);
        }
        foreach (var claim in principal.Claims.ToArray())
        {
            var value = claim.Value;
            if (value.StartsWith('['))
            {
                var values = JsonSerializer.Deserialize<IEnumerable<string>>(value);
                var type = claim.Type;
                foreach (var item in values!)
                {
                    identity.AddClaim(new Claim(type, item));
                }
                identity.RemoveClaim(claim);
            }
        }

        return Task.FromResult(principal);
    }
}
