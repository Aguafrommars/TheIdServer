using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components;

public partial class ThemeButton 
{
    string ThemeCss => ThemeService.Theme == "light" ? "oi-moon" : "oi-sun";

    Task ToggleTheme()
    {
        if (ThemeService.Theme == "dark")
        {
            return ThemeService.SetThemeAsync("light");
        }

        return ThemeService.SetThemeAsync("dark");
    }
}
