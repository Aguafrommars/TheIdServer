using Microsoft.AspNetCore.Blazor.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Blazor.Oidc
{
    public class OidcWebAssemblyHttpMessageHandler : WebAssemblyHttpMessageHandler
    {
        internal new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }

    }
}
