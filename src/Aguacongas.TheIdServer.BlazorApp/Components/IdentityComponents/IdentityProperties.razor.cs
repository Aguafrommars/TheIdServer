using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.IdentityComponents
{
    public partial class IdentityProperties
    {
        protected GridState GridState { get; } = new GridState();

        [Parameter]
        public IdentityResource Model { get; set; }

        [Parameter]
        public EventCallback<IdentityProperty> DeleteEntityClicked { get; set; }

        protected void OnDeleteEntityClicked(IdentityProperty entity)
        {
            DeleteEntityClicked.InvokeAsync(entity);
        }
    }
}
