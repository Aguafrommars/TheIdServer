// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class OneTimeTokenService
    {
        private readonly IAdminStore<OneTimeToken> _store;
        private readonly AuthenticationStateProvider _stateProvider;
        private readonly IAccessTokenProvider _provider;
        private readonly IOptions<ProviderOptions> _options;

        public OneTimeTokenService(IAdminStore<OneTimeToken> store,
            AuthenticationStateProvider state,
            IAccessTokenProvider provider,
            IOptions<ProviderOptions> options)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _stateProvider = state ?? throw new ArgumentNullException(nameof(state));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<string> GetOneTimeToken()
        {
            var tokenResult = await _provider.RequestAccessToken().ConfigureAwait(false);
            tokenResult.TryGetToken(out AccessToken token);
            var state = await _stateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            var oneTimeToken = await _store.CreateAsync(new OneTimeToken
            {
                ClientId = _options.Value.ClientId,
                UserId = state.User.Claims.First(c => c.Type == "sub").Value,
                Data = token.Value,
                Expiration = DateTime.UtcNow.AddMinutes(1)
            }).ConfigureAwait(false);

            return oneTimeToken.Id;
        }
    }
}
