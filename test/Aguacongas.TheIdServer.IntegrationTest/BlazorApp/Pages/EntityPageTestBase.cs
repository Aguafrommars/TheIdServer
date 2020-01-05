using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntityPageTestBase
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
                AuthorizationOptionsExtensions.READER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.All(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public void WhenWriter_should_enable_inputs()
        {
            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.DoesNotContain(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        protected void CreateTestHost(string userName,
            string role,
            string id,
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
                $"http://exemple.com/{Entity}/{id}",
                Fixture.Sut,
                Fixture.TestOutputHelper,
                out host,
                out component,
                out mockHttp);
        }

        protected Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            return Fixture.DbActionAsync(action);
        }

        protected static string GenerateId() => Guid.NewGuid().ToString();

        protected static void WaitForSavedToast(TestHost host, RenderedComponent<App> component)
        {
            WaitForToast("Saved", host, component);
        }

        protected static void WaitForDeletedToast(TestHost host, RenderedComponent<App> component)
        {
            WaitForToast("Deleted", host, component);
        }

        protected static void WaitForToast(string text, TestHost host, RenderedComponent<App> component)
        {
            var toasts = component.FindAll(".toast-body.text-success");
            while (!toasts.Any(t => t.InnerText.Contains(text)))
            {
                host.WaitForNextRender();
                toasts = component.FindAll(".toast-body.text-success");
            }
        }
    }
}