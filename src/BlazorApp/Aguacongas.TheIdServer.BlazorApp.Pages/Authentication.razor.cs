// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Authentication
    {
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
