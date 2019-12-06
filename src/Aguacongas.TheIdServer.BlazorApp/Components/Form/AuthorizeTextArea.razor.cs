using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeTextArea
    {
        [Parameter]
        public string Name { get; set; }
        [Parameter]
        public string Placeholder { get; set; }
    }
}
