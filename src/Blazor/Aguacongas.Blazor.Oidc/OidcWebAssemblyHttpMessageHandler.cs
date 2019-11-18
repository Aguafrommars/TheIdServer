using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebAssembly.Net.Http.HttpClient;

namespace Aguacongas.TheIdServer.Blazor.Oidc
{
    public class OidcWebAssemblyHttpMessageHandler : WasmHttpMessageHandler
    {
        internal new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }

    }
}
