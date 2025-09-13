// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public class ButtonBase : ComponentBase
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
