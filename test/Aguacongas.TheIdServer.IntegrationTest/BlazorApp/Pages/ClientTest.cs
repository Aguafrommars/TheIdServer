using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
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

            var markup = WaitForContains(host, component, "filtered");

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

            var input = WaitForNode(host, component, "#grantTypes input");

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

            var button = WaitForNode(host, component, "#grantTypes div.select");

            host.WaitForNextRender(() => button.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The client should contain at least one grant type.", message.InnerText);
        }

        [Fact]
        public async Task Hybrid_client_should_have_consent()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForNode(host, component, "#consent");

            // hybrid client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // hybrid client should not have device flow lifetime input field
            Assert.Null(component.Find("#device-flow-lifetime"));
            // hybrid client should not have require pkce check box
            Assert.Null(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Code_client_should_have_consent()
        {
            var clientId = await CreateClient("authorization_code");

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForNode(host, component, "#consent");

            // authorization code client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // authorization code client should not have device flow lifetime input field
            Assert.Null(component.Find("#device-flow-lifetime"));
            // authorization code client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Implicit_client_should_have_consent()
        {
            var clientId = await CreateClient("implicit");

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForNode(host, component, "#consent");

            // implicit client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // implicit client should not have device flow lifetime input field
            Assert.Null(component.Find("#device-flow-lifetime"));
            // implicit client should have require pkce check box
            Assert.Null(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Device_client_should_have_consent()
        {
            var clientId = await CreateClient("urn:ietf:params:oauth:grant-type:device_code");

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForNode(host, component, "#consent");

            // device client not should have id token input field
            Assert.Null(component.Find("#id-token"));
            // device client should have device flow lifetime input field
            Assert.NotNull(component.Find("#device-flow-lifetime"));
            // device client should have require pkce check box
            Assert.Null(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Credentials_client_should_not_have_consent()
        {
            var clientId = await CreateClient("client_credentials");

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            // client credentials client should not have consent section
            Assert.Throws<TimeoutException>(() => WaitForNode(host, component, "#consent"));
        }

        [Fact]
        public async Task Password_client_should_not_have_consent()
        {
            var clientId = await CreateClient("password");

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            // resource owner password client should not have consent section
            Assert.Throws<TimeoutException>(() => WaitForNode(host, component, "#consent"));
        }

        [Fact]
        public async Task Custom_client_should_not_have_consent()
        {
            var clientId = await CreateClient("custom");

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForNode(host, component, "#consent");

            // custom client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // custom client should have device flow lifetime input field
            Assert.NotNull(component.Find("#device-flow-lifetime"));
            // custom client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }


        [Fact]
        public async Task DeleteButtonClick_should_delete_client()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#delete-entity input");

            host.WaitForNextRender(() => input.Change(clientId));

            var confirm = component.Find("#delete-entity button.btn-danger");

            host.WaitForNextRender(() => confirm.Click());

            WaitForDeletedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(a => a.Id == clientId);
                Assert.Null(client);
            });

        }

        [Fact]
        public async Task SaveClick_should_create_client()
        {
            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#grantTypes input");

            host.WaitForNextRender(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "authorization_code" }));

            WaitForNode(host, component, "#grantTypes button.dropdown-item");

            var dropdownButton = component.Find("#grantTypes button.dropdown-item");

            host.WaitForNextRender(() => dropdownButton.Click());

            var idInput = component.Find("#id");
            Assert.NotNull(idInput);

            var clientId = GenerateId();
            host.WaitForNextRender(() => idInput.Change(clientId));

            var nameInput = component.Find("#name");
            
            Assert.NotNull(nameInput);
            host.WaitForNextRender(() => nameInput.Change(clientId));

            var form = component.Find("form");
            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                var grantType = await context.ClientGrantTypes.FirstOrDefaultAsync(g => g.ClientId == clientId);
                Assert.NotNull(grantType);
            });
        }

        private async Task<string> CreateClient(string grantType = "hybrid")
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
                        new ClientGrantType{ Id = GenerateId(), GrantType = grantType }
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
