// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientRelyingParty
    {
        private IEnumerable<entity.RelyingParty> _filteredParties;
        private readonly PageRequest _pageRequest = new PageRequest
        {
            Select = nameof(entity.RelyingParty.Id),
            Take = 5
        };

        protected override bool IsReadOnly => false;

        protected override string PropertyName => "Id";

        protected override async Task OnInitializedAsync()
        {
            await GetFilteredValues("", default).ConfigureAwait(false);
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term, CancellationToken cancellationToken)
        {
            _pageRequest.Filter = $"contains({nameof(entity.RelyingParty.Id)},'{term}')";
            var response = await _store.GetAsync(_pageRequest, cancellationToken)
                .ConfigureAwait(false);

            _filteredParties = response.Items;
            return null;
        }

        protected override void SetValue(string inputValue)
        {
            var selected = _filteredParties?.FirstOrDefault(p => p.Id == inputValue);
            CurrentValue = selected?.Id ?? inputValue;
        }

    }
}
