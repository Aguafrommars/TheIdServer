using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntitiesPageTestBase<TEntity>
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

            WaitForLoaded(host, component);

            var markup = component.GetMarkup();
            while (!markup.Contains("table-hover"))
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
                }).ConfigureAwait(false);
                // cancel previous search
                await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = GenerateId()
                }).ConfigureAwait(false);

                await Task.Delay(500).ConfigureAwait(false);
            });


#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
            while ((markup = component.GetMarkup()).Contains("filtered"))
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
            {
                host.WaitForNextRender();
            }


            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task OnRowClicked_should_navigate_to_entity_page()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var markup = WaitForLoaded(host, component);

            Assert.Contains("filtered", markup);

            var tr = component.Find(".table-hover tr");

            Assert.NotNull(tr);

            var navigationManager = host.ServiceProvider.GetRequiredService<TestNavigationManager>();
            bool called = false;
            navigationManager.OnNavigateToCore = (uri, forceload) =>
            {
                called = true;
                Assert.Contains(typeof(TEntity).Name.ToLower(), uri);
            };

            host.WaitForNextRender(() => tr.Click());

            Assert.True(called);
        }

        [Fact]
        public async Task OnHeaderClicked_should_sort_grid()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var markup = WaitForLoaded(host, component);

            Assert.Contains("filtered", markup);

            var th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            host.WaitForNextRender(() => th.Click());

            var arrow = component.Find(".oi-arrow-bottom");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            host.WaitForNextRender(() => th.Click());

            arrow = component.Find(".oi-arrow-top");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            host.WaitForNextRender(() => th.Click());

            arrow = component.Find(".oi-arrow-");

            Assert.NotNull(th);
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

        protected static string WaitForLoaded(TestHost host, RenderedComponent<App> component)
        {
            var markup = component.GetMarkup();

            while (!markup.Contains("filtered"))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            return markup;
        }
    }
}
