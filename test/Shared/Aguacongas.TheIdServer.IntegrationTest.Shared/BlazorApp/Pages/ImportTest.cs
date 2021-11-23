// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop.Infrastructure;
using System.Security.Claims;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ImportTest : TestContext
    {
        public ApiFixture Fixture { get; }

        public ImportTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public void HandleFileSelected_should_report_importation_result()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                out IRenderedComponent<App> component);


            var jsRuntime = Services.GetRequiredService<JSRuntimeImpl>();
            DotNetDispatcher.BeginInvokeDotNet(jsRuntime, new DotNetInvocationInfo(null, "NotifyChange", 1, default), "[[{ \"name\": \"test.json\" }]]");

            Assert.Contains("text-success", component.Markup);
        }

        private void CreateTestHost(string userName,
           string role,
           out IRenderedComponent<App> component)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role)
                },
                Fixture.Sut,
                this,
                out component);
            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            navigationManager.NavigateTo("import");
        }
    }
}
