using Aguacongas.TheIdServer.BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    public class NavMenuTest
    {
        [Fact]
        public void ToggleNavMenu_should_update_nav_menu_class()
        {
            var navigationInterceptionMock = new Mock<INavigationInterception>();

            var host = new TestHost();
            host.ConfigureServices(services =>
            {
                services.AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var component = host.AddComponent<NavMenu>();

            var div = component.Find("div.collapse");
            Assert.NotNull(div);

            host.WaitForNextRender(() => div.Click());

            Assert.Null(component.Find("div.collapse"));
        }
    }
}
