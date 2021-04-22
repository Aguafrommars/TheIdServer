﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class IdentityProvider
    {
        private bool _isReadOnly;
        private IEnumerable<entity.ExternalProvider> _filteredProviders;
        private string _providerName;
        private readonly PageRequest _pageRequest = new PageRequest
        {
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Provider";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Id != null;
        }

        protected override async Task OnInitializedAsync()
        {
            _providerName = await GetProviderName(Entity.Provider).ConfigureAwait(false);
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term, CancellationToken cancellationToken)
        {
            _pageRequest.Filter = $"contains({nameof(entity.ExternalProvider.Id)},'{term}') or contains({nameof(entity.ExternalProvider.DisplayName)},'{term}')";
            var response = await _store.GetAsync(_pageRequest, cancellationToken)
                .ConfigureAwait(false);

            _filteredProviders = response.Items;
            return null;
        }

        protected override void SetValue(string inputValue)
        {
            var selected = _filteredProviders?.FirstOrDefault(p => p.Id == inputValue);
            _providerName = selected?.DisplayName ?? inputValue;
            Entity.Provider = selected?.Id ?? inputValue;
        }

        private async Task<string> GetProviderName(string id)
        {
            var provider = await _store.GetAsync(id, null)
                .ConfigureAwait(false);

            return provider.DisplayName;
        }
    }
}
