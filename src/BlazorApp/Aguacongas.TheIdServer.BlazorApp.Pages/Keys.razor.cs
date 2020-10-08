using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    [Authorize(Policy = "Is4-Reader")]
    public partial class Keys
    {
        private ICollection<Key> _dataProtectionKeys;
        private ICollection<Key> _signingKeys;

        protected override async Task OnInitializedAsync()
        {
            await GetDataProtectionKeys().ConfigureAwait(false);
            await GetSingingKeys().ConfigureAwait(false);
            await base.OnInitializedAsync();
        }

        private async Task GetSingingKeys()
        {
            var signingKeysResponse = await _signingKeyStore.GetAllKeysAsync().ConfigureAwait(false);
            _signingKeys = signingKeysResponse.Items.ToList();
        }

        private async Task GetDataProtectionKeys()
        {
            var _dataProtectionKeysResponse = await _dataProtectionKeyStore.GetAllKeysAsync().ConfigureAwait(false);
            _dataProtectionKeys = _dataProtectionKeysResponse.Items.ToList();
        }

        private async Task RevokeDataProtectionKey(Tuple<string, string> tuple)
        {
            await _dataProtectionKeyStore.RevokeKeyAsync(tuple.Item1, tuple.Item2);
            await GetDataProtectionKeys().ConfigureAwait(false);
        }


        private async Task RevokeSigningKey(Tuple<string, string> tuple)
        {
            await _signingKeyStore.RevokeKeyAsync(tuple.Item1, tuple.Item2);
            await GetSingingKeys().ConfigureAwait(false);
        }
    }
}
