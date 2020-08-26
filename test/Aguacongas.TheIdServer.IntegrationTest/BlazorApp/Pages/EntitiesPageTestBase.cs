// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntitiesPageTestBase<TEntity> : IDisposable
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
                SharedConstants.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            host.WaitForContains(component, "filtered");

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


            string markup;
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
            while ((markup = component.GetMarkup()).Contains("filtered"))
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
            {
                host.WaitForNextRender();
            }


            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task Export_click_should_download_entities()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            await host.WaitForNextRenderAsync(() => selectAll.ChangeAsync(true));

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");

            Assert.NotNull(button);

            var provider = host.ServiceProvider;
            var runtime = provider.GetRequiredService<Mock<IJSRuntime>>();
            string calledUrl = null;
            Task<HttpResponseMessage> calledTask = null;
            runtime.Setup(m => m.InvokeAsync<object>("open", It.IsAny<object[]>()))
                .Callback<string, object[]>((m, p) => {
                    calledUrl = p[0].ToString();
                    calledTask = provider.GetRequiredService<HttpClient>().GetAsync(calledUrl);
                    })
                .Returns(new ValueTask<object>(calledTask));
            
            await host.WaitForNextRenderAsync(() => button.ClickAsync());

            Assert.NotNull(calledUrl);
            var response = await calledTask.ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var testUserService = _fixture.Sut.Services.GetRequiredService<TestUserService>();
            testUserService.User = null;
            response = await provider.GetRequiredService<HttpClient>().GetAsync(calledUrl);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task OnRowClicked_should_navigate_to_entity_page()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var markup = WaitForLoaded(host, component);

            Assert.Contains("filtered", markup);

            var tdList = component.FindAll(".table-hover tr td").ToArray();

            var navigationManager = host.ServiceProvider.GetRequiredService<TestNavigationManager>();
            bool called = false;
            navigationManager.OnNavigateToCore = (uri, forceload) =>
            {
                called = true;
                Assert.Contains(typeof(TEntity).Name.ToLower(), uri);
            };

            await host.WaitForNextRenderAsync(() => tdList[1].ClickAsync());

            Assert.True(called);
        }

        [Fact]
        public async Task OnHeaderClicked_should_sort_grid()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var markup = WaitForLoaded(host, component);

            Assert.Contains("filtered", markup);

            var th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            await host.WaitForNextRenderAsync(() => th.ClickAsync());

            var arrow = component.Find(".oi-arrow-bottom");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            await host.WaitForNextRenderAsync(() => th.ClickAsync());

            arrow = component.Find(".oi-arrow-top");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            await host.WaitForNextRenderAsync(() => th.ClickAsync());

            arrow = component.Find(".oi-arrow-");

            Assert.NotNull(arrow);
        }

        [Fact]
        public async Task OnSelectAllClicked_should_select_all_items()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var markup = WaitForLoaded(host, component);

            Assert.Contains("filtered", markup);

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            await host.WaitForNextRenderAsync(() => selectAll.ChangeAsync(true));

            var selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            selectAll = selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            await host.WaitForNextRenderAsync(() => selectAll.ChangeAsync(false));

            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.DoesNotContain(selected.Attributes, a => a.Name == "checked");

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            await host.WaitForNextRenderAsync(() => selected.ChangeAsync(true));

            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");
        }

        protected abstract Task PopulateList();

        protected void CreateTestHost(string userName,
            string role,
            out TestHost host,
            out RenderedComponent<App> component,
            out MockHttpMessageHandler mockHttp)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READER),
                    new Claim("role", role),
                    new Claim("sub", Guid.NewGuid().ToString())
                },
                $"http://exemple.com/{Entities}",
                _fixture.Sut,
                _fixture.TestOutputHelper,
                out host,
                out component,
                out mockHttp);
            _host = host;
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
