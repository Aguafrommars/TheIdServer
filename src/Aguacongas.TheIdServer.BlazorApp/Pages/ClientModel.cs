using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public class ClientModel: ComponentBase
    {
        private bool _sortById = false;

        [Inject]
        public HttpClient HttpClient { get; set; }

        protected IEnumerable<Entity.Client> ClientList { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var page = await HttpClient.GetJsonAsync<PageResponse<Entity.Client>>("/api/client?take=10")
                .ConfigureAwait(false);
            ClientList = page.Items;
        }

        protected async Task SortById()
        {
            var sortClose = "Id";
            if (_sortById)
            {
                sortClose += " desc";
            }
            _sortById = !_sortById;
            var page = await HttpClient.GetJsonAsync<PageResponse<Entity.Client>>($"/api/client?orderby={sortClose}&take=10")
                .ConfigureAwait(false);
            ClientList = page.Items;
        }
    }
}
