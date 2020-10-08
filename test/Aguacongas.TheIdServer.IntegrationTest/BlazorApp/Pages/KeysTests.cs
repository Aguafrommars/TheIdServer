using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components.Testing;
using RichardSzalay.MockHttp;
using System.Security.Claims;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class KeysTests
    {
        private TestHost _host;
        public ApiFixture Fixture { get; }

        public KeysTests(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public void OnInitializeAsync_should_load_all_keys()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out RenderedComponent<App> component);

            var markup = _host.WaitForContains(component, "creation");
            Assert.Contains("Signing", markup);
        }


        private void CreateTestHost(string userName,
           string role,
           out RenderedComponent<App> component)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READER),
                    new Claim("role", role)
                },
                $"http://exemple.com/keys",
                Fixture.Sut,
                Fixture.TestOutputHelper,
                out TestHost host,
                out component,
                out MockHttpMessageHandler _,
                true);
            _host = host;
        }

        private void WaitForLoaded(RenderedComponent<App> component)
        {
            var markup = component.GetMarkup();

            while (!markup.Contains("Keys"))
            {
                _host.WaitForNextRender();
                markup = component.GetMarkup();
            }
        }
    }
}
