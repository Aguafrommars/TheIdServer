// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntityPageTestBase<TComnponent> : TestContext where TComnponent : IComponent
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
                out IRenderedComponent<TComnponent> component);

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
                out IRenderedComponent<TComnponent> component);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.DoesNotContain(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        protected void CreateTestHost(string userName,
            string role,
            string id,
            out IRenderedComponent<TComnponent> component)
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

        protected Task DbActionAsync<TContext>(Func<TContext, Task> action) where TContext : DbContext
        {
            return Fixture.DbActionAsync(action);
        }

        protected static string GenerateId() => Guid.NewGuid().ToString();


        protected static IElement WaitForNode(IRenderedComponent<TComnponent> component, string cssSelector)
        => component.WaitForElement(cssSelector);

        protected static List<IElement> WaitForAllNodes(IRenderedComponent<TComnponent> component, string cssSelector)
        => component.WaitForElements(cssSelector).ToList();
    }
}