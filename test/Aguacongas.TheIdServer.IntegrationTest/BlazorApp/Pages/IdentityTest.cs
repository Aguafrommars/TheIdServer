using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class IdentityTest : EntityPageTestBase
    {
        public override string Entity => "identityresource";
        public IdentityTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }



        [Fact]
        public async Task OnFilterChanged_should_filter_properties_and_claims()
        {
            var identityId = GenerateId();
            await DbActionAsync<IdentityServerDbContext>(context =>
            {
                context.Identities.Add(new IdentityResource
                {
                    Id = identityId,
                    DisplayName = identityId,
                    IdentityClaims = new List<IdentityClaim>
                    {
                        new IdentityClaim { Id = GenerateId(), Type = "filtered" }
                    },
                    Properties = new List<IdentityProperty>
                    {
                        new IdentityProperty { Id = GenerateId(), Key = "filtered", Value = "filtered" }
                    }
                });

                return context.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                identityId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender();

            var markup = component.GetMarkup();

            if (markup.Contains("Loading..."))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            markup = component.GetMarkup();

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = identityId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }
    }
}
