using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class LoginDisplay
    {
        private async Task BeginSignOut(MouseEventArgs args)
        {
            await _signOutManager.SetSignOutState();
            _navigationManager.NavigateTo("authentication/logout");
        }
    }
}
