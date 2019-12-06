using Microsoft.AspNetCore.Identity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Users
    {
        protected override string SelectProperties => "Id,UserName";

        protected override void OnRowClicked(IdentityUser entity)
        {
            NavigationManager.NavigateTo($"user/{entity.Id}");
        }
    }
}
