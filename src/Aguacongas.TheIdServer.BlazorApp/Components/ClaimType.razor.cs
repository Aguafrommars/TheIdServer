using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class ClaimType : AutoCompleteModel<IClaimType>
    {
        [Inject]
        IAdminStore<IdentityClaim> Store { get; set; }

        private readonly PageRequest _pageRequest = new PageRequest
        {
            Select = "Type",
            Take = 5
        };

        protected override bool IsReadOnly => !string.IsNullOrEmpty(Entity.Type);

        protected override async Task<IEnumerable<string>> GetFilteredValues(string term)
        {
            _pageRequest.Filter = $"contains(Type,'{term}')";
            var response = await Store.GetAsync(_pageRequest)
                .ConfigureAwait(false);

            return response.Items.Select(c => c.Type);
        }

        protected override void SetValue(string inputValue)
        {
            Entity.Type = inputValue;
        }
    }
}
