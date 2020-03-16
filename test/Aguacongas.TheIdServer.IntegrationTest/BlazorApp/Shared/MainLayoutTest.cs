using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static Aguacongas.TheIdServer.IntegrationTest.TestUtils;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    [Collection("api collection")]

    public class MainLayoutTest : IDisposable
    {
        private readonly ApiFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public MainLayoutTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task WhenNoAuthorized_should_display_message()
        {
            TestUtils.CreateTestHost(
                "test",
                Array.Empty<Claim>(),
                "http://exemple.com/clients",
                _fixture.Sut,
                _testOutputHelper,
                out TestHost host,
                out MockHttpMessageHandler mockHttp);
            var provider = host.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();
            var state = await provider.GetAuthenticationStateAsync();
            var idendity = state.User.Identity as FakeIdendity;
            idendity.SetIsAuthenticated(false);

            var component = host.AddComponent<App>();

            var markup = component.GetMarkup();
            Assert.Contains("You're not authorized to reach this page.", markup);
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
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READER)
                },
                $"http://exemple.com/clients",
                _fixture.Sut,
                _fixture.TestOutputHelper,
                out TestHost host,
                out RenderedComponent<blazorApp.App> component,
                out MockHttpMessageHandler mockHttp);
            _host = host;

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
