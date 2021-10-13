// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop.Infrastructure;
using RichardSzalay.MockHttp;
using System.Security.Claims;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ImportTest
    {
        private TestHost _host;
        public ApiFixture Fixture { get; }

        public ImportTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Do not pass any more")]
        public void HandleFileSelected_should_report_importation_result()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                out RenderedComponent<App> component);

            WaitForLoaded(component);

            var jsRuntime = _host.ServiceProvider.GetRequiredService<JSRuntimeImpl>();
            DotNetDispatcher.BeginInvokeDotNet(jsRuntime, new DotNetInvocationInfo(null, "NotifyChange", 1, default), "[[{ \"name\": \"test.json\" }]]");

            var markup = _host.WaitForContains(component, "text-success");
            Assert.Contains("text-success", markup);
        }

        private void CreateTestHost(string userName,
           string role,
           out RenderedComponent<App> component)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role)
                },
                $"http://exemple.com/import",
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

            while (!markup.Contains("Import"))
            {
                _host.WaitForNextRender();
                markup = component.GetMarkup();
            }
        }        
    }


}
