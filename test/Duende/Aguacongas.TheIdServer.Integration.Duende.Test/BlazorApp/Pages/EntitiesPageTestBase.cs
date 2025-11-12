using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Pages;
using Bunit;
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

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntitiesPageTestBase<TEntity, TComponent> : BunitContext 
        where TComponent : EntitiesModel<TEntity>
        where TEntity: class
    {
        private readonly TheIdServerFactory _factory;

        public abstract string Entities { get; }

        protected virtual string FilteredString => "filtered";

        protected EntitiesPageTestBase(TheIdServerFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_entities()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            await component.WaitForStateAsync(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = GenerateId()
            });
            // cancel previous search
            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = GenerateId()
            });

            await component.WaitForStateAsync(() => !component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            Assert.DoesNotContain(FilteredString, component.Markup);
        }

        [Fact]
        public async Task Export_click_should_download_entities()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            await component.WaitForStateAsync(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            component.Render();
            await (await component.InvokeAsync(() => component.Find(".table.mb-0 th input"))).ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });

            button = await component.InvokeAsync(() => component.Find("button.btn-secondary"));
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");

            Assert.NotNull(button);

            var invocationHanlder = JSInterop.SetupVoid("open", i => true).SetVoidResult();

            await button.ClickAsync(new MouseEventArgs());

            Assert.Single(invocationHanlder.Invocations);
            var calledUrl = invocationHanlder.Invocations.First().Arguments[0] as string;

            var testUserService = _factory.Services.GetRequiredService<TestUserService>();
            testUserService.User = null;
            var response = await Services.GetRequiredService<HttpClient>().GetAsync(calledUrl);
            AssertExportResponse(response);
        }


        [Fact]
        public async Task OnRowClicked_should_navigate_to_entity_page()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            await component.WaitForStateAsync(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            var navigationManager = Services.GetRequiredService<NavigationManager>();

            await component.InvokeAsync(() =>
            {
                var tdList = component.FindAll(".table-hover tr td");
                Assert.NotEmpty(tdList);
                tdList[tdList.Count - 1].Click(new MouseEventArgs());
            });

            Assert.Contains(typeof(TEntity).Name.ToLower(), navigationManager.Uri);
        }

        [Fact]
        public async Task OnHeaderClicked_should_sort_grid()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            await component.WaitForStateAsync(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

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

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            await component.WaitForStateAsync(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            Assert.NotNull(component.Find(".table.mb-0 th input"));

            await component.WaitForStateAsync(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));
            component.Render();

            await (await component.InvokeAsync(() => component.Find(".table.mb-0 th input"))).ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });
            component.Render();
            var selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");
            var input = await component.InvokeAsync(() => component.Find(".table.mb-0 th input"));
            Assert.NotNull(input);

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = false
            });
            component.Render();
            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.DoesNotContain(selected.Attributes, a => a.Name == "checked");

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            await (await component.InvokeAsync(() => component.Find(".table.mb-0 th input"))).ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });
            component.Render();
            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");
        }

        protected abstract Task PopulateList();

        protected virtual void AssertExportResponse(HttpResponseMessage response)
        {
            Assert.False(response.IsSuccessStatusCode);
        }

        protected IRenderedComponent<TComponent> CreateComponent(string userName,
            string role)
        {
            _factory.ConfigureTestContext(userName,
                [
                    new Claim("scope", SharedConstants.ADMINSCOPE),
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role),
                    new Claim("sub", Guid.NewGuid().ToString())
                ],
                this);

            var component = Render<TComponent>();
            component.WaitForState(() => !component.Markup.Contains("Loading..."), TimeSpan.FromMinutes(1));
            return component;
        }

        protected Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            return _factory.DbActionAsync(action);
        }

        protected static string GenerateId() => Guid.NewGuid().ToString();
    }
}
