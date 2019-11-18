using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public class ApiEntitesGridModel<T> : ComponentBase
    {
        protected GridState GridState { get; } = new GridState();

        [Parameter]
        public ProtectResource Model { get; set; }

        [Parameter]
        public EventCallback<T> DeleteEntityClicked { get; set; }

        protected void OnDeleteEntityClicked(T entity)
        {
            DeleteEntityClicked.InvokeAsync(entity);
        }
    }
}
