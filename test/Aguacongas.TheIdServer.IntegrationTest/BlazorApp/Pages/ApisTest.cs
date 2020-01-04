using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ApisTest
    {
        private readonly ApiFixture _fixture;

        public ApisTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_apis()
        {
            await _fixture.DbActionAsync<IdentityServerDbContext>(context =>
            {
                context.Apis.Add(new ProtectResource
                {
                    Id = "filteder",
                    DisplayName = "filtered"
                });

                return context.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender(() =>
            {
            });

            var markup = component.GetMarkup();

            Assert.Contains("Loading...", markup);

            host.WaitForNextRender();

            markup = component.GetMarkup();

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(() => {
                filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = GenerateId()
                });
                // cancel previous search
                filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = GenerateId()
                });
            });

            markup = component.GetMarkup();

            host.WaitForNextRender(() => Thread.Sleep(500));

            markup = component.GetMarkup();

            host.WaitForNextRender(() => Thread.Sleep(500));

            markup = component.GetMarkup();
            Assert.DoesNotContain("filtered", markup);
        }


        private void CreateTestHost(string userName,
            string role,
            out TestHost host,
            out RenderedComponent<App> component,
            out MockHttpMessageHandler mockHttp)
        {
            TestUtils.CreateTestHost(userName,
                new List<SerializableClaim>
                {
                    new SerializableClaim
                    {
                        Type = "role",
                        Value = AuthorizationOptionsExtensions.READER
                    },
                    new SerializableClaim
                    {
                        Type = "role",
                        Value = role
                    }
                },
                $"http://exemple.com/apis", 
                _fixture.Sut,
                _fixture.TestOutputHelper,
                out host,
                out component,
                out mockHttp);
        }

        private static string GenerateId() => Guid.NewGuid().ToString();
    }
}
