// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using AngleSharp.Dom;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntityPageTestBase<T> : TestContext where T: IComponent
    {
        public abstract string Entity { get; }

        public ApiFixture Fixture { get; }

        protected EntityPageTestBase(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestOutputHelper = testOutputHelper;
        }


        [Fact]
        public void WhenNonWriter_should_disable_inputs()
        {
            CreateTestHost("Bob Smith",
                SharedConstants.READERPOLICY,
                null,
                out IRenderedComponent<T> component);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.All(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public void WhenWriter_should_enable_inputs()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null,
                out IRenderedComponent<T> component);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.DoesNotContain(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        protected void CreateTestHost(string userName,
            string role,
            string id,
            out IRenderedComponent<T> component)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[] 
                {
                    new Claim("scope", SharedConstants.ADMINSCOPE),
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role) 
                },
                Fixture.Sut,
                this,
                out component,
                ComponentParameter.CreateParameter("Id", id));
            var c = component;
            c.WaitForState(() => !c.Markup.Contains("Loading..."));
        }

        protected Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            return Fixture.DbActionAsync(action);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Fixture.Dispose();
            }
        }
        protected static string GenerateId() => Guid.NewGuid().ToString();


        protected static IElement WaitForNode(IRenderedComponent<T> component, string cssSelector)
        => component.WaitForElement(cssSelector);

        protected static List<IElement> WaitForAllNodes(IRenderedComponent<T> component, string cssSelector)
        => component.WaitForElements(cssSelector).ToList();
    }
}