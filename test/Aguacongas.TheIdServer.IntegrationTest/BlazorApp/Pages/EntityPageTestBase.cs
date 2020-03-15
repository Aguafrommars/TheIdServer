using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public abstract class EntityPageTestBase : IDisposable
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
                SharedConstants.READER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.All(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public void WhenWriter_should_enable_inputs()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
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
                new Claim[] 
                {
                    new Claim("role", SharedConstants.READER),
                    new Claim("role", role) 
                },
                $"http://exemple.com/{Entity}/{id}",
                Fixture.Sut,
                Fixture.TestOutputHelper,
                out host,
                out component,
                out mockHttp);
            _host = host;
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

        protected static string WaitForLoaded(TestHost host, RenderedComponent<App> component)
        {
            var markup = component.GetMarkup();

            while (markup.Contains("Authentication in progress") || markup.Contains("Loading..."))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            return markup;
        }

        protected static HtmlNode WaitForNode(TestHost host, RenderedComponent<App> component, string selector)
        {
            return host.WaitForNode(component, selector);
        }

        protected static ICollection<HtmlNode> WaitForAllNodes(TestHost host, RenderedComponent<App> component, string selector)
        {
            return host.WaitForAllNodes(component, selector);
        }


        protected static string WaitForContains(TestHost host, RenderedComponent<App> component, string term)
        {
            return host.WaitForContains(component, term);
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
                    _host.Dispose();
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