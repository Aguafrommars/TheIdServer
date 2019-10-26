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

            var page = await AdminStore.GetAsync(new PageRequest
            {
                Take = 10
            }).ConfigureAwait(false);
            ClientList = page.Items;
            
            GridState.OnHeaderClicked += async e =>
            {
                var p = await AdminStore.GetAsync(new PageRequest
                {
                    OrderBy = e.OrderBy,
                    Take = 10
                }).ConfigureAwait(false);;
                ClientList = p.Items;
                StateHasChanged();
            };
        }
    }
}
