using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Pages;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class CloneButton
    {
        [Parameter]
        public string CssClass { get; set; }

        private Task Clone()
        {
            Navigation.NavigateTo(Navigation.GetUriWithQueryParameter(nameof(EntityModel<Client>.Clone), true));
            return Task.CompletedTask;
        }
    }
}
