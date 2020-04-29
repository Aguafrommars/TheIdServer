using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntitiesGrid<TItem>
    {
        [Parameter]
        public string TableClass { get; set; } = "table";

        [Parameter]
        public string HeaderRowClass { get; set; } = "select";

        [Parameter]
        public string RowClass { get; set; } = "select";

        [Parameter]
        public string FooterClass { get; set; }

        [Parameter]
        public RenderFragment TableHeader { get; set; }

        [Parameter]
        public RenderFragment<TItem> RowTemplate { get; set; }

        [Parameter]
        public RenderFragment TableFooter { get; set; }

        [Parameter]
        public IEnumerable<TItem> Items { get; set; }

        [Parameter]
        public GridState GridState { get; set; } = new GridState();

        [Parameter]
        public EventCallback<TItem> RowClicked { get; set; }
    }
}
