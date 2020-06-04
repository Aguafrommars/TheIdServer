using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntityResourceTranslatation
    {
        [Parameter]
        public IEntityResource Resource { get; set; }

        [Parameter]
        public EventCallback<IEntityResource> DeleteResource { get; set; }
        protected override void OnInitialized()
        {
            Console.WriteLine($"Resource = {Resource}");
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        private Task OnDeleteClick(MouseEventArgs args)
        {
            return DeleteResource.InvokeAsync(Resource);
        }
    }
}
