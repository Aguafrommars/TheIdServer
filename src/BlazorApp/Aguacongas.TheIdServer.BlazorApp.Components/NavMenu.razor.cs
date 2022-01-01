// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class NavMenu
    {
        [Inject]
        private IOptions<MenuOptions> Options { get; set; }

        bool collapseNavMenu = true;

        string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}