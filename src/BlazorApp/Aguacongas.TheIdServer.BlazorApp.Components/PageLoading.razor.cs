// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre

using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class PageLoading
    {
        [Parameter]
        public string Information { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
