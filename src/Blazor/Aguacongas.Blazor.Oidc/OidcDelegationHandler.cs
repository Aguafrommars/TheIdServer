using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Http;

namespace Aguacongas.Blazor.Oidc
{
    public class OidcDelegationHandler : DelegatingHandler
    {
        private readonly OidcWebAssemblyHttpMessageHandler _parent;

        public OidcDelegationHandler(OidcWebAssemblyHttpMessageHandler parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _parent.SendAsync(request, cancellationToken);
        }
    }
}
