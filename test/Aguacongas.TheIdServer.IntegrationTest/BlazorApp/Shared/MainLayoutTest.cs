using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    public class MainLayoutTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MainLayoutTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WhenNoAuthorized_should_display_message()
        {
            var component = CreateComponent("test");

            var markup = component.GetMarkup();
            Assert.Contains("You're not authorize to use this application.", markup);
        }

        [Fact]
        public void WhenAuthorized_should_display_welcome_message()
        {
            var expected = "Bob Smith";
            var component = CreateComponent(expected);

            var markup = component.GetMarkup();
            Assert.Contains(expected, markup);
        }

        private RenderedComponent<App> CreateComponent(string userName)
        {
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var host = new TestHost();
            _host = host;
            host.ConfigureServices(services =>
            {
                blazorApp.Program.ConfigureServices(services);
                services.AddLogging(configure =>
                {
                    configure.AddProvider(new TestLoggerProvider(_testOutputHelper));
                })
                    .AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");

            var component = host.AddComponent<App>();
            var markup = component.GetMarkup();
            Assert.Contains("Authentication in progress", markup);

            return component;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private TestHost _host;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _host?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
