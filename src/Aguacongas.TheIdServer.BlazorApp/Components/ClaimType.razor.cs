using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class ClaimType
    {
        private bool _isReadOnly;

        private readonly PageRequest _pageRequest = new PageRequest
        {
            Select = nameof(IClaimType.Type),
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Type";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Type != null;
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            _pageRequest.Filter = $"contains({nameof(Entity.Type)},'{term}')";
            var response = await _store.GetAsync(_pageRequest)
                .ConfigureAwait(false);

            return response.Items.Select(c => c.Type);
        }

        protected override void SetValue(string inputValue)
        {
            Entity.Type = inputValue;
        }
    }
}
