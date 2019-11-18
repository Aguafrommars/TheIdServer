using Microsoft.AspNetCore.Blazor.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Blazor.Oidc
{
    public class OidcDelegationHandler : DelegatingHandler
    {
        private readonly IUserStore _userStore;
        private readonly OidcWebAssemblyHttpMessageHandler _innerHandler;

        public OidcDelegationHandler(IUserStore userStore, OidcWebAssemblyHttpMessageHandler innerHandler)
        {
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(_userStore.AuthenticationScheme, _userStore.AccessToken);
            request.Properties[WebAssemblyHttpMessageHandler.FetchArgs] = new
            {
                credentials = "include"
            };
            return _innerHandler.SendAsync(request, cancellationToken);
        }
    }
}
