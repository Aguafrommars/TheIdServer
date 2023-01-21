// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer
{
    public class OAuthDelegatingHandler: OAuthDelegatingHandler<IdentityServerOptions>
    {
        public OAuthDelegatingHandler(OAuthTokenManager manager, HttpMessageHandler innerHandler)
            : base(manager, innerHandler)
        { }

        public OAuthDelegatingHandler(OAuthTokenManager manager)
            : base(manager)
        { }
    }

    public class OAuthDelegatingHandler<T> : DelegatingHandler where T: IdentityServerOptions, new()
    {
        private readonly OAuthTokenManager<T> _manager;

        public OAuthDelegatingHandler(OAuthTokenManager<T> manager, HttpMessageHandler innerHandler):base(innerHandler)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public OAuthDelegatingHandler(OAuthTokenManager<T> manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _manager.GetTokenAsync().ConfigureAwait(false);
            request.Headers.Authorization = accessToken;
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
