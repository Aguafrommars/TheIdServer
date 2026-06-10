using Duende.AccessTokenManagement.DPoP;
using Microsoft.AspNetCore.Authentication;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Extensions;

public static class HttpContextExtensions
{
    const string AuthenticationPropertiesDPoPKey = ".Token.dpop_proof_key";

    internal static DPoPProofKey? GetProofKey(this AuthenticationProperties properties)
    {
        if (properties.Items.TryGetValue(AuthenticationPropertiesDPoPKey, out var key))
        {
            if (key == null)
            {
                return null;
            }

            return DPoPProofKey.Parse(key);
        }

        return null;
    }

    const string HttpContextDPoPKey = "dpop_proof_key";

    internal static void SetCodeExchangeDPoPKey(this HttpContext context, DPoPProofKey dpopProofKey) =>
        context.Items[HttpContextDPoPKey] = dpopProofKey;
}
