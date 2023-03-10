using Aguacongas.TheIdServer.BlazorApp.Components;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Aguacongas.TheIdServer.IntegrationTest.BlazorApp;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Integration.Duende.Test.BlazorApp.Components;

[Collection(BlazorAppCollection.Name)]
public class ThemeButtonTest : TestContext
{
    [Fact]
    public void Click_should_toggle_theme()
    {
        var expected = "dark";
        Services.AddSingleton<ThemeService>();
        var invocation = JSInterop.SetupVoid("setTheme", expected);

        var cut = RenderComponent<ThemeButton>();

        var button = cut.Find("button");

        var themeService = cut.Services.GetRequiredService<ThemeService>();
        themeService.ThemeChanged += (o, s) =>
        {
            Assert.Equal(expected, themeService.Theme);
        };

        button.Click(new MouseEventArgs());
        invocation.SetVoidResult();
    }
}
