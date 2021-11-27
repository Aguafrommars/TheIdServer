// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntitiesPageTestBase<TEntity, TComponent> : TestContext where TComponent: IComponent
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
                SharedConstants.WRITERPOLICY,
                out IRenderedComponent<TComponent> component);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

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

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task Export_click_should_download_entities()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                out IRenderedComponent<TComponent> component);

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            await selectAll.ChangeAsync(new ChangeEventArgs
            {
                Value = true
            }).ConfigureAwait(false);

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");

            Assert.NotNull(button);

            var invocationHanlder = JSInterop.SetupVoid("open", i => true).SetVoidResult();
            
            await button.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            Assert.Single(invocationHanlder.Invocations);
            var calledUrl = invocationHanlder.Invocations.First().Arguments[0] as string;

            var testUserService = _fixture.Sut.Services.GetRequiredService<TestUserService>();
            testUserService.User = null;
            var response = await Services.GetRequiredService<HttpClient>().GetAsync(calledUrl);
            Assert.False(response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task OnRowClicked_should_navigate_to_entity_page()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                out IRenderedComponent<TComponent> component);

            Assert.Contains("filtered", component.Markup);

            var tdList = component.FindAll(".table-hover tr td").ToArray();

            var navigationManager = Services.GetRequiredService<NavigationManager>();

            await tdList[1].ClickAsync(new MouseEventArgs());

            Assert.Contains(typeof(TEntity).Name.ToLower(), navigationManager.Uri);
        }

        [Fact]
        public async Task OnHeaderClicked_should_sort_grid()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                out IRenderedComponent<TComponent> component);

            Assert.Contains("filtered", component.Markup);

            var th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            await th.ClickAsync(new MouseEventArgs());

            var arrow = component.Find(".oi-arrow-bottom");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            await th.ClickAsync(new MouseEventArgs());

            arrow = component.Find(".oi-arrow-top");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            await th.ClickAsync(new MouseEventArgs());

            arrow = component.Find(".oi-arrow-");

            Assert.NotNull(arrow);
        }

        [Fact]
        public async Task OnSelectAllClicked_should_select_all_items()
        {
            await PopulateList();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                out IRenderedComponent<TComponent> component);

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            await selectAll.ChangeAsync(new ChangeEventArgs
            {
                Value = true
            }).ConfigureAwait(false);

            var selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            await selectAll.ChangeAsync(new ChangeEventArgs
            {
                Value = false
            }).ConfigureAwait(false);

            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.DoesNotContain(selected.Attributes, a => a.Name == "checked");

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            await selectAll.ChangeAsync(new ChangeEventArgs
            {
                Value = true
            }).ConfigureAwait(false);

            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");
        }

        protected abstract Task PopulateList();

        protected void CreateTestHost(string userName,
            string role,
            out IRenderedComponent<TComponent> component)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("scope", SharedConstants.ADMINSCOPE),
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role),
                    new Claim("sub", Guid.NewGuid().ToString())
                },
                _fixture.Sut,
                this,
                out component);

            var c = component;
            c.WaitForState(() => !c.Markup.Contains("Loading..."), TimeSpan.FromMinutes(1));
        }

        protected Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            return _fixture.DbActionAsync(action);
        }

        protected static string GenerateId() => Guid.NewGuid().ToString();
    }
}
