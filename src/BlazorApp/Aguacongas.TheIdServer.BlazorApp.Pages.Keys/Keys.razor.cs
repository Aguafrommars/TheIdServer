﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Keys
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
            await RevokeKey(_dataProtectionKeyStore, tuple.Item1, tuple.Item2).ConfigureAwait(false);
            await GetDataProtectionKeys().ConfigureAwait(false);
        }


        private async Task RevokeSigningKey(Tuple<string, string> tuple)
        {
            await RevokeKey(_signingKeyStore, tuple.Item1, tuple.Item2).ConfigureAwait(false);
            await GetSingingKeys().ConfigureAwait(false);
        }

        private async Task RevokeKey<T>(IKeyStore<T> keyStore, string id, string reason)
        {
            try
            {
                await keyStore.RevokeKeyAsync(id, reason).ConfigureAwait(false);
                await _notifier.NotifyAsync(new Notification
                {
                    Header = id,
                    Message = Localizer["Revoked"]
                }).ConfigureAwait(false);
            }
            catch
            {
                await _notifier.NotifyAsync(new Notification
                {
                    Header = id,
                    IsError = true,
                    Message = Localizer["Error when trying to revoke the key."]
                }).ConfigureAwait(false);
            }
        }
    }
}
