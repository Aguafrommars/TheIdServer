using Aguacongas.IdentityServer.Store;
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
    public abstract class EntitiesPageTestBase<TEntity, TComponent> : TestContext where TComponent : IComponent
    {
        private readonly TheIdServerFactory _factory;

        public abstract string Entities { get; }


        protected EntitiesPageTestBase(TheIdServerFactory factory)
        {
            _factory = factory;
        }

        [Fact(Skip = "Fail often on appveyor")]
        public async Task OnFilterChanged_should_filter_entities()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            component.WaitForState(() => !component.Markup.Contains("filtered"), TimeSpan.FromMinutes(1));

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            filterInput.TriggerEvent("oninput", new ChangeEventArgs
            {
                Value = GenerateId()
            });
            // cancel previous search
            filterInput.TriggerEvent("oninput", new ChangeEventArgs
            {
                Value = GenerateId()
            });

            await Task.Delay(500).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task Export_click_should_download_entities()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            component.WaitForState(() => component.Markup.Contains("filtered"), TimeSpan.FromMinutes(1));

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            selectAll.Change(new ChangeEventArgs
            {
                Value = true
            });

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");

            Assert.NotNull(button);

            var invocationHanlder = JSInterop.SetupVoid("open", i => true).SetVoidResult();

            button.Click(new MouseEventArgs());

            Assert.Single(invocationHanlder.Invocations);
            var calledUrl = invocationHanlder.Invocations.First().Arguments[0] as string;

            var testUserService = _factory.Services.GetRequiredService<TestUserService>();
            testUserService.User = null;
            var response = await Services.GetRequiredService<HttpClient>().GetAsync(calledUrl);
            Assert.False(response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task OnRowClicked_should_navigate_to_entity_page()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            component.WaitForState(() => component.Markup.Contains("filtered"), TimeSpan.FromMinutes(1));

            var navigationManager = Services.GetRequiredService<NavigationManager>();

            var tdList = component.FindAll(".table-hover tr td");

            Assert.NotEmpty(tdList);            
           
            tdList[tdList.Count - 1].Click(new MouseEventArgs());

            Assert.Contains(typeof(TEntity).Name.ToLower(), navigationManager.Uri);
        }

        [Fact]
        public async Task OnHeaderClicked_should_sort_grid()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            component.WaitForState(() => component.Markup.Contains("filtered"), TimeSpan.FromMinutes(1));

            var th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            th.Click(new MouseEventArgs());

            var arrow = component.Find(".oi-arrow-bottom");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            th.Click(new MouseEventArgs());

            arrow = component.Find(".oi-arrow-top");

            Assert.NotNull(arrow);

            th = component.Find(".table.mb-0 th div");

            Assert.NotNull(th);

            th.Click(new MouseEventArgs());

            arrow = component.Find(".oi-arrow-");

            Assert.NotNull(arrow);
        }

        [Fact]
        public async Task OnSelectAllClicked_should_select_all_items()
        {
            await PopulateList();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            component.WaitForState(() => component.Markup.Contains("filtered"), TimeSpan.FromMinutes(1));

            var selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            selectAll.Change(new ChangeEventArgs
            {
                Value = true
            });

            var selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            selectAll = component.Find(".table.mb-0 th input");

            Assert.NotNull(selectAll);

            selectAll.Change(new ChangeEventArgs
            {
                Value = false
            });

            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.DoesNotContain(selected.Attributes, a => a.Name == "checked");

            var button = component.Find("button.btn-secondary");
            Assert.Contains(button.Attributes, a => a.Name == "disabled");

            selectAll.Change(new ChangeEventArgs
            {
                Value = true
            });

            selected = component.Find(".table.table-hover td input");

            Assert.NotNull(selected);
            Assert.Contains(selected.Attributes, a => a.Name == "checked");

            button = component.Find("button.btn-secondary");
            Assert.DoesNotContain(button.Attributes, a => a.Name == "disabled");
        }

        protected abstract Task PopulateList();

        protected IRenderedComponent<TComponent> CreateComponent(string userName,
            string role)
        {
            _factory.ConfigureTestContext(userName,
                new Claim[]
                {
                    new Claim("scope", SharedConstants.ADMINSCOPE),
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role),
                    new Claim("sub", Guid.NewGuid().ToString())
                },
                this);

            var component = RenderComponent<TComponent>();
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
