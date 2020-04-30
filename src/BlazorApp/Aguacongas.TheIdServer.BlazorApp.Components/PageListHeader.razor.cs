using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class PageListHeader
    {
        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string Name { get; set; }
    }
}
