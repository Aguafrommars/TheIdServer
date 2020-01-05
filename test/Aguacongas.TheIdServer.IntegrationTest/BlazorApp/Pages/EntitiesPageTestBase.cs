using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntitiesPageTestBase
    {
        private readonly ApiFixture _fixture;

        public abstract string Entities { get; }

        protected EntitiesPageTestBase(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_entities()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender(() =>
            {
            });

            var markup = component.GetMarkup();

            if (markup.Contains("Loading..."))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => {
                await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = GenerateId()
                });
                // cancel previous search
                await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = GenerateId()
                });
            });


#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
            while ((markup = component.GetMarkup()).Contains("filtered"))
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
            {
                host.WaitForNextRender();
            }


            Assert.DoesNotContain("filtered", markup);
        }

        protected abstract Task PopulateList();

        protected void CreateTestHost(string userName,
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
                $"http://exemple.com/{Entities}",
                _fixture.Sut,
                _fixture.TestOutputHelper,
                out host,
                out component,
                out mockHttp);
        }

        protected Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            return _fixture.DbActionAsync(action);
        }

        protected static string GenerateId() => Guid.NewGuid().ToString();

    }
}
