using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public class EntityModel<T>: ComponentBase where T: class
    {
        [Inject]
        public IAdminStore<T> AdminStore { get; set; }

        [Inject]
        public GridState GridState { get; set; }

        protected IEnumerable<T> EntityList { get; private set; }

        protected virtual string SelectProperties { get; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            var pageRequest = new PageRequest
            {
                Select = SelectProperties,
                Take = 10
            };
            await GetClientList(pageRequest)
                .ConfigureAwait(false);

            GridState.OnHeaderClicked += async e =>
            {
                pageRequest.OrderBy = e.OrderBy;
                await GetClientList(pageRequest)
                    .ConfigureAwait(false);
                StateHasChanged();
            };
        }

        private async Task GetClientList(PageRequest pageRequest)
        {
            var page = await AdminStore.GetAsync(pageRequest)
                            .ConfigureAwait(false);
            EntityList = page.Items;
        }
    }
}
