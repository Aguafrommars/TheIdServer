// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityNS = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientRelyingParty
    {
        private IEnumerable<EntityNS.RelyingParty> _filteredParties;
        private readonly PageRequest _pageRequest = new PageRequest
        {
            Select = nameof(EntityNS.RelyingParty.Id),
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
            _pageRequest.Filter = $"contains({nameof(EntityNS.RelyingParty.Id)},'{term}')";
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
