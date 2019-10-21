using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
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
        [Inject]
        public HttpClient HttpClient { get; set; }

        protected IEnumerable<Entity.Client> ClientList { get; private set; }

        protected string CurrentSortedProperty { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var page = await HttpClient.GetJsonAsync<PageResponse<Entity.Client>>("/api/client?take=10")
                .ConfigureAwait(false);
            ClientList = page.Items;
        }

        protected async Task Sort(SortEventArgs e)
        {
            CurrentSortedProperty = e.PropertyName;
            var page = await HttpClient.GetJsonAsync<PageResponse<Entity.Client>>($"/api/client?orderby={e.OrderBy}&take=10")
                .ConfigureAwait(false);
            ClientList = page.Items;
        }
    }
}
