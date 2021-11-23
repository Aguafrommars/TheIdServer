// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    [Collection("api collection")]

    public class MainLayoutTest : TestContext
    {
        private readonly ApiFixture _fixture;
     
        public MainLayoutTest(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task WhenNotAuthenticated_should_redirect_to_login()
        {
            TestUtils.CreateTestHost(
                "test",
                Array.Empty<Claim>(),
                _fixture.Sut,
                this,
                out IRenderedComponent<App> component);

            var provider = Services.GetRequiredService<AuthenticationStateProvider>();
            var state = await provider.GetAuthenticationStateAsync();
            var idendity = state.User.Identity as TestUtils.FakeIdendity;
            idendity.SetIsAuthenticated(false);

            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            navigationManager.NavigateTo("clients");

            Assert.DoesNotContain("Clients.", component.Markup);
        }

        [Fact]
        public void WhenNotAuthaurized_should_displax_message()
        {
            TestUtils.CreateTestHost(
                "test",
                Array.Empty<Claim>(),
                _fixture.Sut,
                this,
                out IRenderedComponent<App> component);

            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            navigationManager.NavigateTo("clients");

            Assert.Contains("Your are not authorized to view this page.", component.Markup);
        }

        [Fact]
        public void WhenAuthorized_should_display_welcome_message()
        {
            var expected = "Bob Smith";
            TestUtils.CreateTestHost(
                expected,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READERPOLICY)
                },
                _fixture.Sut,
                this,
                out IRenderedComponent<App> component);

            Assert.Contains(expected, component.Markup);
        }
    }
}
