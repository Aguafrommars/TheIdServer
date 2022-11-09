// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Authentication
    {
        private RemoteAuthenticatorView _remoteAuthenticatorView;

        [Parameter]
        public string Action { get; set; }
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
