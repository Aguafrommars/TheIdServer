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
    public class ClientTest : EntityPageTestBase
    {
        public override string Entity => "client";
        public ClientTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }



        [Fact]
        public async Task OnFilterChanged_should_filter_properties_scopes_claims_and_secret()
        {
            var clientId = GenerateId();
            await DbActionAsync<IdentityServerDbContext>(context =>
            {
                context.Clients.Add(new Client
                {
                    Id = clientId,
                    ClientName = clientId,
                    ProtocolType = "oidc",
                    AllowedGrantTypes = new List<ClientGrantType> 
                    { 
                        new ClientGrantType{ Id = GenerateId(), GrantType = "hybrid" }
                    },
                    AllowedScopes = new List<ClientScope>
                    {
                        new ClientScope{ Id = GenerateId(), Scope = "filtered"}
                    },
                    RedirectUris = new List<ClientUri>
                    {
                        new ClientUri{ Id = GenerateId(), Uri = "http://filtered", Kind = 1 }
                    },
                    ClientClaims = new List<ClientClaim>
                    {
                        new ClientClaim { Id = GenerateId(), Type = "filtered", Value="filtered" }
                    },
                    ClientSecrets = new List<ClientSecret> 
                    {
                        new ClientSecret{ Id= GenerateId(), Type = "SHA256", Value = "filtered", Description = "filtered"}
                    },
                    Properties = new List<ClientProperty>
                    {
                        new ClientProperty { Id = GenerateId(), Key = "filtered", Value = "filtered" }
                    },
                    IdentityProviderRestrictions = new List<ClientIdpRestriction>
                    {
                        new ClientIdpRestriction{ Id = GenerateId(), Provider = "Google"}
                    }                   
                });

                return context.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
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
                Value = clientId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }
    }
}
