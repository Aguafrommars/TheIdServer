using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeText
    {
        [Parameter]
        public string Name { get; set; }
        [Parameter]
        public string Placeholder { get; set; }
    }
}
