using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public class ClientEntitiesGridModel<T> : ComponentBase
    {
        protected GridState GridState { get; } = new GridState();

        [Parameter]
        public Client Model { get; set; }

        [Parameter]
        public EventCallback<T> DeleteEntityClicked { get; set; }

        protected void OnDeleteEntityClicked(T entity)
        {
            DeleteEntityClicked.InvokeAsync(entity);
        }
    }
}
