using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntitySubGridTitle
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public EventCallback AddClicked { get; set; }
    }
}
