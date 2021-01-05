// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class PageListHeader
    {
        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public PageRequest ExportRequest { get; set; }

        [Parameter]
        public bool ExportDisabled { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
