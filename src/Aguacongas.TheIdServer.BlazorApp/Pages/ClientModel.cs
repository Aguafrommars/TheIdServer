using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public class ClientModel: ComponentBase
    {
        [Inject]
        public IAdminStore<Entity.Client> AdminStore { get; set; }

        [Inject]
        public GridState GridState { get; set; }

        protected IEnumerable<Entity.Client> ClientList { get; private set; }

        protected string CurrentSortedProperty { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            var pageRequest = new PageRequest
            {
                Select = "Id, ClientName, Description",
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
            ClientList = page.Items;
        }
    }
}
