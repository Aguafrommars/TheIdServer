using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
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
        public async Task WhenNotAuthenticated_should_redirect_to_login()
        {
            TestUtils.CreateTestHost(
                "test",
                Array.Empty<Claim>(),
                "http://exemple.com/clients",
                _fixture.Sut,
                _testOutputHelper,
                out TestHost host,
                out MockHttpMessageHandler mockHttp);
            var serviceProvider = host.ServiceProvider;
            
            var provider = serviceProvider.GetRequiredService<AuthenticationStateProvider>();
            var state = await provider.GetAuthenticationStateAsync();
            var idendity = state.User.Identity as TestUtils.FakeIdendity;
            idendity.SetIsAuthenticated(false);

            var component = host.AddComponent<App>();

            var markup = component.GetMarkup();
            Assert.DoesNotContain("Clients.", markup);
        }

        [Fact]
        public void WhenNotAuthaurized_should_displax_message()
        {
            TestUtils.CreateTestHost(
                "test",
                Array.Empty<Claim>(),
                "http://exemple.com/clients",
                _fixture.Sut,
                _testOutputHelper,
                out TestHost host,
                out MockHttpMessageHandler _);

            var component = host.AddComponent<App>();

            var markup = component.GetMarkup();
            Assert.Contains("Your are not authorized to view this page.", markup);
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
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);
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
