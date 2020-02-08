using Aguacongas.TheIdServer.BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    public class NavMenuTest
    {
        [SuppressMessage("Critical Code Smell", "S4487:Unread \"private\" fields should be removed", Justification = "To enable console log")]
        [SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "To enable console log")]
        private readonly ITestOutputHelper _testOutputHelper;

        public NavMenuTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ToggleNavMenu_should_update_nav_menu_class()
        {
            var navigationInterceptionMock = new Mock<INavigationInterception>();

            using var host = new TestHost();
            host.ConfigureServices(services =>
            {
                services.AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var component = host.AddComponent<NavMenu>();

            var div = component.Find("div.collapse");
            Assert.NotNull(div);

            await host.WaitForNextRenderAsync(() => div.ClickAsync());

            Assert.Null(component.Find("div.collapse"));
        }
    }
}
