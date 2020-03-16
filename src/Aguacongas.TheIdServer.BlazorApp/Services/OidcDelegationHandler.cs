using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp
{
    public class OidcDelegationHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpMessageHandler _innerHanler;
        private readonly MethodInfo _method;

        public OidcDelegationHandler(IAccessTokenProvider accessTokenProvider, HttpMessageHandler innerHanler)
        {
            _accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
            _innerHanler = innerHanler ?? throw new ArgumentNullException(nameof(innerHanler));
            var type = innerHanler.GetType();
            _method = type.GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod) ?? throw new InvalidOperationException("Cannot get SendAsync method");
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: Remove this when httpclient support per request fetch options
            WebAssemblyHttpMessageHandlerOptions.DefaultCredentials = FetchCredentialsOption.Include;

            var tokenResult = await _accessTokenProvider.RequestAccessToken().ConfigureAwait(false);
            tokenResult.TryGetToken(out var token);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            var task = _method.Invoke(_innerHanler, new object[] { request, cancellationToken }) as Task<HttpResponseMessage>;
            return await task.ConfigureAwait(false);
        }
    }
}
