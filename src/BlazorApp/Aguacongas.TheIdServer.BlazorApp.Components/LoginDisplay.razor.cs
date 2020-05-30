using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class LoginDisplay
    {
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
        private async Task BeginSignOut(MouseEventArgs args)
        {
            await _signOutManager.SetSignOutState();
            _navigationManager.NavigateTo("authentication/logout");
        }
    }
}
