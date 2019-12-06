using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeButton
    {
        [Parameter]
        public string CssSubClass { get; set; }

        [Parameter]
        public string Type { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> Clicked { get; set; }
    }
}
