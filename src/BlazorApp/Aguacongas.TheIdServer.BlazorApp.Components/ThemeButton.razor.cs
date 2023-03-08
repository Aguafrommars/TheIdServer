using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components;

public partial class ThemeButton 
{
    string ThemeCss => ThemeService.Theme == "light" ? "oi-moon" : "oi-sun";

    async Task ToggleTheme()
    {
        if (ThemeService.Theme == "dark")
        {
            await ThemeService.SetThemeAsync("light").ConfigureAwait(false);
            return;
        }

        await ThemeService.SetThemeAsync("dark").ConfigureAwait(false);
    }
}
