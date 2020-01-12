using Aguacongas.IdentityServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components.UserComponents
{
    public partial class UserRole
    {
        private bool _isReadOnly;
        private readonly PageRequest _pageRequest = new PageRequest
        {
            Select = "Name",
            Take = 5
        };

        protected override bool IsReadOnly => _isReadOnly;

        protected override string PropertyName => "Name";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _isReadOnly = Entity.Name != null;
        }

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            _pageRequest.Filter = $"contains(Name,'{term}')";
            var response = await _store.GetAsync(_pageRequest)
                .ConfigureAwait(false);

            return response.Items.Select(r => r.Name);
        }

        protected override void SetValue(string inputValue)
        {
            Entity.Id = Guid.NewGuid().ToString();
            Entity.Name = inputValue;
        }
    }
}
