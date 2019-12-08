using Microsoft.AspNetCore.Identity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Roles
    {
        protected override string SelectProperties => "Id,Name";

        protected override void OnRowClicked(IdentityRole entity)
        {
            NavigationManager.NavigateTo($"role/{entity.Id}");
        }
    }
}
