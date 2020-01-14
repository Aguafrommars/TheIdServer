using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using RichardSzalay.MockHttp;
using System;
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
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var markup = component.GetMarkup();

            while (!markup.Contains("filtered"))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = clientId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task AddGrantType_should_validate_grant_type_rules()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = component.Find("#grantTypes input");

            while(input == null)
            {
                host.WaitForNextRender();
                input = component.Find("#grantTypes input");
            }

            host.WaitForNextRender(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "test test" }));

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The grant type cannot contains space.", message.InnerText);

            input = component.Find("#grantTypes input");
            Assert.NotNull(input);
            host.WaitForNextRender(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "hybrid" }));

            message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The grant type must be unique.", message.InnerText);

            input = component.Find("#grantTypes input");
            Assert.NotNull(input);

            host.WaitForNextRender(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "authorization_code" }));

            message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("&#x27;Code&#x27; cannot be added to a client with grant type &#x27;Hybrid&#x27;.", message.InnerText);

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var messages = component.FindAll(".validation-message");

            Assert.Contains(messages, m => m.InnerText.Contains("&#x27;Hybrid&#x27; cannot be added to a client with grant type &#x27;Implicit&#x27;."));
        }

        [Fact]
        public async Task RemoveGrantType_should_validate_grant_type_rule()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var button = component.Find("#grantTypes div.select");

            while(button == null)
            {
                host.WaitForNextRender();
                button = component.Find("#grantTypes div.select");
            }

            host.WaitForNextRender(() => button.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The client should contain at least one grant type.", message.InnerText);
        }

        private async Task<string> CreateClient()
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
            return clientId;
        }
    }
}
