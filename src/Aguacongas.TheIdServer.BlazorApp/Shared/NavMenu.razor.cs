// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.BlazorApp.Shared
{
    public partial class NavMenu
    {
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