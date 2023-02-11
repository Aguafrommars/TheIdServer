// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class EntitiesGrid<TItem>
    {
        [Parameter]
        public bool SetKey { get; set; } = true;

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
        public EventCallback<TItem> RowClicked { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        private ICollection<TItem> GetItems => new List<TItem>(Items);
    }
}
