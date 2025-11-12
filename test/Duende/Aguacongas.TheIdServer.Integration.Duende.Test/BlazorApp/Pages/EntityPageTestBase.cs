// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
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

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntityPageTestBase<TComnponent> : BunitContext where TComnponent : IComponent
    {
        public abstract string Entity { get; }

        public TheIdServerFactory Factory { get; }

        protected EntityPageTestBase(TheIdServerFactory factory)
        {
            Factory = factory;
        }

        [Fact]
        public void WhenWriter_should_enable_inputs()
        {
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.DoesNotContain(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        protected IRenderedComponent<TComnponent> CreateComponent(string userName,
            string role,
            string? id,
            bool clone = false)
        {
            Factory.ConfigureTestContext(userName,
               new Claim[]
               {
                    new Claim("scope", SharedConstants.ADMINSCOPE),
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role),
                    new Claim("sub", Guid.NewGuid().ToString())
               },
               this);

            var component = Render<TComnponent>(builder => builder.AddUnmatched("Id", id).AddUnmatched("Clone", clone));
            component.WaitForState(() => !component.Markup.Contains("Loading..."), TimeSpan.FromMinutes(1));
            return component;
        }

        protected Task DbActionAsync<TContext>(Func<TContext, Task> action) where TContext : DbContext
        {
            return Factory.DbActionAsync(action);
        }

        protected static string GenerateId() => Guid.NewGuid().ToString();


        protected static IElement WaitForNode(IRenderedComponent<TComnponent> component, string cssSelector)
        => component.WaitForElement(cssSelector);

        protected static List<IElement> WaitForAllNodes(IRenderedComponent<TComnponent> component, string cssSelector)
        => component.WaitForElements(cssSelector).ToList();
    }
}