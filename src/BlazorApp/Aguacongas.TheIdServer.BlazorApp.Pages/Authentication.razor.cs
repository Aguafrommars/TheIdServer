using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Authentication
    {
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
