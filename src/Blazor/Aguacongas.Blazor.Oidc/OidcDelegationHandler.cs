using Microsoft.AspNetCore.Blazor.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Blazor.Oidc
{
    public class OidcDelegationHandler : DelegatingHandler
    {
        private readonly IUserStore _userStore;
        private readonly HttpMessageHandler _innerHanler;
        private readonly MethodInfo _method;

        public OidcDelegationHandler(IUserStore userStore, HttpMessageHandler innerHanler)
        {
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _innerHanler = innerHanler ?? throw new ArgumentNullException(nameof(innerHanler));
            var type = innerHanler.GetType();
            _method = type.GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod) ?? throw new InvalidOperationException("Cannot get SendAsync method");
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: Remove this when httpclient support per request fetch options
            WebAssemblyHttpMessageHandlerOptions.DefaultCredentials = FetchCredentialsOption.Include;
            request.Headers.Authorization = new AuthenticationHeaderValue(_userStore.AuthenticationScheme, _userStore.AccessToken);

            return _method.Invoke(_innerHanler, new object[] { request, cancellationToken }) as Task<HttpResponseMessage>;
        }
    }
}
