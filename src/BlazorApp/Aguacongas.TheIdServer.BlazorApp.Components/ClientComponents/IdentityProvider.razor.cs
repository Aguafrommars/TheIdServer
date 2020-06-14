using Aguacongas.IdentityServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class IdentityProvider
    {
        private bool _isReadOnly;
        private IEnumerable<entity.IdentityProvider> _filteredProviders;
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
            _isReadOnly = Entity.Provider != null;
        }

        protected override async Task OnInitializedAsync()
        {
            _providerName = await GetProviderName(Entity.Provider).ConfigureAwait(false);
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            _pageRequest.Filter = $"contains({nameof(entity.IdentityProvider.Id)},'{term}') or contains({nameof(entity.IdentityProvider.DisplayName)},'{term}')";
            var response = await _store.GetAsync(_pageRequest)
                .ConfigureAwait(false);

            _filteredProviders = response.Items;
            StateHasChanged();
            return null;
        }

        protected override void SetValue(string inputValue)
        {
            _providerName = _filteredProviders?.FirstOrDefault(p => p.Id == inputValue)?.DisplayName ?? inputValue;
        }

        private async Task<string> GetProviderName(string id)
        {
            var provider = await _store.GetAsync(id)
                .ConfigureAwait(false);

            return provider.DisplayName;
        }
    }
}
