using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.AspNetCore.Components.Web;
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
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            host.WaitForContains(component, "filtered");

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = clientId
            }));

            var markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task AddGrantType_should_validate_grant_type_rules()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#grantTypes input");

            await host.WaitForNextRenderAsync(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "test test" }));

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The grant type cannot contains space.", message.InnerText);

            input = component.Find("#grantTypes input");
            Assert.NotNull(input);
            await host.WaitForNextRenderAsync(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "hybrid" }));

            message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The grant type must be unique.", message.InnerText);

            input = component.Find("#grantTypes input");
            Assert.NotNull(input);

            await host.WaitForNextRenderAsync(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "authorization_code" }));

            message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("&#x27;Code&#x27; cannot be added to a client with grant type &#x27;Hybrid&#x27;.", message.InnerText);

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());
        }

        [Theory]
        [InlineData("0.01:00:00")]
        [InlineData("01:00:00")]
        [InlineData("01:00")]
        [InlineData("15")]
        [InlineData("15d")]
        [InlineData("15h")]
        [InlineData("15m")]
        public async Task AddToken_should_validate_token_regex(string value)
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#access-token");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync("test test"));

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.StartsWith("The token expression doesn&#x27;t match a valid format.", message.InnerText);

            input = component.Find("#access-token");
            await host.WaitForNextRenderAsync(() => input.ChangeAsync(value));

            message = component.Find(".validation-message");

            Assert.Null(message);
        }

        [Fact]
        public async Task ScopeInputChange_should_filter_scopes_list()
        {
            string clientId = await CreateClient();

            var firstId = GenerateId();
            var secondId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(async c =>
            {
                await c.Apis.AddAsync(new ProtectResource
                {
                    Id = firstId,
                    DisplayName = firstId,
                    Scopes = new List<ApiScope>
                    {
                        new ApiScope
                        {
                            Id = firstId,
                            Scope = firstId,
                            DisplayName = firstId
                        }
                    }
                });

                await c.Identities.AddAsync(new IdentityResource
                {
                    Id = secondId,
                    DisplayName = secondId,                    
                });

                await c.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var expected = 1;
            var input = WaitForNode(host, component, "#scopes input");

            await host.WaitForNextRenderAsync(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = firstId }));

            var nodes = host.WaitForAllNodes(component, "#scopes .dropdown-item");

            Assert.Equal(expected, nodes.Count);

            await host.WaitForNextRenderAsync(() => nodes.First().ClickAsync());

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var scope = await context.ClientScopes.FirstOrDefaultAsync(s => s.ClientId == clientId && s.Scope == firstId);
                Assert.NotNull(scope);
            });
        }



        [Fact]
        public async Task RemoveGrantType_should_validate_grant_type_rule()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var button = WaitForNode(host, component, "#grantTypes div.select");

            await host.WaitForNextRenderAsync(() => button.ClickAsync());

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The client should contain at least one grant type.", message.InnerText);
        }

        [Fact]
        public async Task Add_delete_click_test()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var deleteButtons = WaitForAllNodes(host, component,"span.oi-trash").ToList();
            var addButtons = WaitForAllNodes(host, component, "button.btn.btn-sm.btn-primary.ml-md-auto").ToList();

            for(int i = 0; i < addButtons.Count; i++)
            {
                await host.WaitForNextRenderAsync(() => addButtons[i].ClickAsync());
                addButtons = WaitForAllNodes(host, component, "button.btn.btn-sm.btn-primary.ml-md-auto").ToList();
            }

            deleteButtons = WaitForAllNodes(host, component, "span.oi-trash").ToList();

            do
            {
                await host.WaitForNextRenderAsync(() => deleteButtons.Last().ParentNode.ClickAsync());
                deleteButtons = WaitForAllNodes(host, component, "span.oi-trash").ToList();
            } while (deleteButtons.Count > 1);

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());
        }

        [Fact]
        public async Task Hybrid_client_should_have_consent()
        {
            string clientId = await CreateClient();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);

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
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);

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
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);

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
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);

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
                SharedConstants.WRITER,
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
                SharedConstants.WRITER,
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
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);

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
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#delete-entity input");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(clientId));

            var confirm = component.Find("#delete-entity button.btn-danger");

            await host.WaitForNextRenderAsync(() => confirm.ClickAsync());

            WaitForDeletedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(a => a.Id == clientId);
                Assert.Null(client);
            });

        }

        [Fact]
        public async Task SaveClick_should_create_client()
        {
            string clientId = GenerateId();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#grantTypes input");

            host.WaitForNextRender(() => input.TriggerEventAsync("onfocus", new FocusEventArgs()));

            host.WaitForNoRender();

            var button = WaitForNode(host, component, "#grantTypes .dropdown-item.m-0.p-0.pl-1.pr-1");
            
            host.WaitForNextRender(() => button.ClickAsync());

            var idInput = component.Find("#id");
            Assert.NotNull(idInput);

            await host.WaitForNextRenderAsync(() => idInput.ChangeAsync(clientId));

            var nameInput = component.Find("#name");
            
            Assert.NotNull(nameInput);
            await host.WaitForNextRenderAsync(() => nameInput.ChangeAsync(clientId));

            var form = component.Find("form");
            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
            });
        }

        [Fact]
        public async Task SaveClick_should_sanetize_cors_uri()
        {
            string clientId = await CreateClient("authorization_code");
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                clientId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#urls input[name=\"cors\"]");

            host.WaitForNextRender(() => input.ChangeAsync(true));

            var form = component.Find("form");
            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.Include(c => c.RedirectUris)
                    .FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                var uri = client.RedirectUris.FirstOrDefault(u => (u.Kind & UriKinds.Cors) == UriKinds.Cors);
                Assert.NotNull(uri);
                Assert.Equal("HTTP://FILTERED:80", uri.SanetizedCorsUri);
            });

            input = WaitForNode(host, component, "#urls input[name=\"cors\"]");

            host.WaitForNextRender(() => input.ChangeAsync(false));

            form = component.Find("form");
            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.Include(c => c.RedirectUris)
                    .FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                var uri = client.RedirectUris.FirstOrDefault();
                Assert.NotNull(uri);
                Assert.Null(uri.SanetizedCorsUri);
            });
        }

        private async Task<string> CreateClient(string grantType = "hybrid")
        {
            var clientId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(context =>
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
                        new ClientUri{ Id = GenerateId(), Uri = "http://filtered", Kind = UriKinds.Redirect }
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
                    },
                    Resources = new List<ClientLocalizedResource>
                    {
                        new ClientLocalizedResource
                        {
                            Id = GenerateId(),
                            ClientId = clientId,
                            CultureId = "en",
                            ResourceKind = EntityResourceKind.DisplayName,
                            Value = GenerateId()
                        }
                    }
                });

                return context.SaveChangesAsync();
            });
            return clientId;
        }
    }
}
