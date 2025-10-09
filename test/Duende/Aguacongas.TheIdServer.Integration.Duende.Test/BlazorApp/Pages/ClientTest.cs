﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Integration.Duende.Test;
using AngleSharp.Dom;
using Bunit;
using Bunit.Extensions.WaitForHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ClientPage = Aguacongas.TheIdServer.BlazorApp.Pages.Client.Client;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ClientTest : EntityPageTestBase<ClientPage>
    {
        public override string Entity => "client";
        public ClientTest(TheIdServerFactory factory) : base(factory)
        {
        }


        [Fact]
        public async Task OnFilterChanged_should_filter_properties_scopes_claims_and_secret()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = clientId
            });

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task UrlsHeaderClick_should_sort_urls()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var header = component.Find("#urls th div");

            Assert.NotNull(header);

            await header.ClickAsync(new MouseEventArgs());

            var urls = component.FindAll("#urls tr");

            var firstUrl = urls.ToArray()[1];

            header = component.Find("#urls th div");

            await header.ClickAsync(new MouseEventArgs());

            urls = component.FindAll("#urls tr");

            Assert.NotEqual(firstUrl.ToDiffMarkup(), urls.ToArray()[1].ToDiffMarkup());

            var headers = component.FindAll("#urls th div");

            await headers.ToArray()[1].ClickAsync(new MouseEventArgs());

            urls = component.FindAll("#urls tr");

            Assert.Equal(firstUrl.ToDiffMarkup(), urls.ToArray()[1].ToDiffMarkup());

            headers = component.FindAll("#urls th div");

            await headers.ToArray()[1].ClickAsync(new MouseEventArgs());

            urls = component.FindAll("#urls tr");

            Assert.NotEqual(firstUrl.ToDiffMarkup(), urls.ToArray()[1].ToDiffMarkup());
        }

        [Fact]
        public async Task AddGrantType_should_validate_grant_type_rules()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var input = component.Find("#grantTypes input");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "test test" });

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The grant type cannot contains space.", message.ToMarkup());

            input = component.Find("#grantTypes input");
            Assert.NotNull(input);
            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "hybrid" });

            message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The grant type must be unique.", message.ToMarkup());

            input = component.Find("#grantTypes input");
            Assert.NotNull(input);

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "authorization_code" });

            message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("'Code' cannot be added to a client with grant type 'Hybrid'.", message.ToMarkup());

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync();
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

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);
            
            var input = component.Find("#access-token");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = "test test"
            });

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.StartsWith("The token expression doesn't match a valid format.", message.InnerHtml);

            input = component.Find("#access-token");
            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = value
            });

            Assert.Throws<ElementNotFoundException>(()=> component.Find(".validation-message"));
        }

        [Fact]
        public async Task ScopeInputChange_should_filter_scopes_list()
        {
            string clientId = await CreateClient();

            var firstId = GenerateId();
            var secondId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(async c =>
            {
                await c.ApiScopes.AddAsync(new ApiScope
                {
                    Id = firstId,
                    DisplayName = firstId,
                });

                await c.Identities.AddAsync(new IdentityResource
                {
                    Id = secondId,
                    DisplayName = secondId,                    
                });

                await c.SaveChangesAsync();
            });

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var expected = 1;
            var input = WaitForNode(component, "#scopes input");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = firstId });

            var nodes = WaitForAllNodes(component, "#scopes .dropdown-item");

            Assert.Equal(expected, nodes.Count);

            await nodes.First().ClickAsync(new MouseEventArgs());

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var scope = await context.ClientScopes.FirstOrDefaultAsync(s => s.ClientId == clientId && s.Scope == firstId);
                Assert.NotNull(scope);
            });
        }

        [Fact]
        public async Task AlgorithmInputChanges_should_filter_scopes_list()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var expected = 3;
            var input = WaitForNode(component, "#tokens input.new-claim");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "RS" });

            var nodes = WaitForAllNodes(component, "#tokens .list-inline-item .dropdown-item");

            Assert.Equal(expected, nodes.Count);
        }


        [Fact]
        public async Task RemoveGrantType_should_validate_grant_type_rule()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var button = WaitForNode(component, "#grantTypes div.select");

            await button.ClickAsync(new MouseEventArgs());

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync();

            var message = component.Find(".validation-message");

            Assert.NotNull(message);
            Assert.Contains("The client should contain at least one grant type.", message.ToMarkup());
        }

        [Fact]
        public async Task Hybrid_client_should_have_consent()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#consent");

            // hybrid client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // hybrid client should not have device flow lifetime input field
            Assert.Throws<ElementNotFoundException>(() => component.Find("#device-flow-lifetime"));
            // hybrid client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Code_client_should_have_consent()
        {
            var clientId = await CreateClient("authorization_code");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#consent");

            // authorization code client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // authorization code client should not have device flow lifetime input field
            Assert.Throws<ElementNotFoundException>(() => component.Find("#device-flow-lifetime"));
            // authorization code client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Implicit_client_should_have_consent()
        {
            var clientId = await CreateClient("implicit");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#consent");

            // implicit client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // implicit client should not have device flow lifetime input field
            Assert.Throws<ElementNotFoundException>(() => component.Find("#device-flow-lifetime"));
            // implicit client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Device_client_should_have_consent()
        {
            var clientId = await CreateClient("urn:ietf:params:oauth:grant-type:device_code");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#consent");

            // device client should have device flow lifetime input field
            Assert.NotNull(component.Find("#device-flow-lifetime"));
            // device client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }

        [Fact]
        public async Task Credentials_client_should_not_have_consent()
        {
            var clientId = await CreateClient("client_credentials");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            // client credentials client should not have consent section
            Assert.Throws<WaitForFailedException>(() => WaitForNode(component, "#consent"));
        }

        [Fact]
        public async Task Credentials_client_should_not_have_identity_scopes()
        {
            var clientId = await CreateClient("client_credentials");

            var firstId = $"t{GenerateId()}";
            var secondId = $"t{GenerateId()}";
            await DbActionAsync<ConfigurationDbContext>(async c =>
            {
                c.ApiScopes.RemoveRange(c.ApiScopes);
                await c.ApiScopes.AddAsync(new ApiScope
                {
                    Id = firstId,
                    DisplayName = firstId,
                });

                c.Identities.RemoveRange(c.Identities);
                await c.Identities.AddAsync(new IdentityResource
                {
                    Id = secondId,
                    DisplayName = secondId,
                });

                await c.SaveChangesAsync();
            });

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var expected = 1;
            var input = WaitForNode(component, "#scopes input");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "t" });

            var nodes = WaitForAllNodes(component, "#scopes .dropdown-item");

            Assert.Equal(expected, nodes.Count);
        }

        [Fact]
        public async Task Password_client_should_not_have_consent()
        {
            var clientId = await CreateClient("password");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            // resource owner password client should not have consent section
            Assert.Throws<WaitForFailedException>(() => WaitForNode(component, "#consent"));
        }

        [Fact]
        public async Task Ciba_client_should_have_ciba_lifetime()
        {
            var clientId = await CreateClient("urn:openid:params:grant-type:ciba");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#ciba-lifetime");

            // ciba client should have polling interval input field
            Assert.NotNull(component.Find("#polling-interval"));
        }


        [Fact]
        public async Task Custom_client_should_not_have_consent()
        {
            var clientId = await CreateClient("custom");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#consent");

            // custom client should have id token input field
            Assert.NotNull(component.Find("#id-token"));
            // custom client should have device flow lifetime input field
            Assert.NotNull(component.Find("#device-flow-lifetime"));
            // custom client should have require pkce check box
            Assert.NotNull(component.Find("input[name=require-pkce]"));
        }

        [SkipCiFact]
        public async Task Options_clicks_should_enable_save_button()
        {
            var clientId = await CreateClient(allowOfflineAccess: true);

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            WaitForNode(component, "#tokens");

            var optionButton = component.Find("input[name=access-token-type]");
            optionButton.Change(true);

            optionButton = component.Find("input[name=refresh-token-usage]");
            optionButton.Change(true);

            optionButton = component.Find("input[name=refresh-token-expiration]");
            optionButton.Change(true);

            var saveButton = component.Find("button[type=submit]");

            Assert.False(saveButton.IsDisabled());
        }


        [Fact]
        public async Task DeleteButtonClick_should_delete_client()
        {
            string clientId = await CreateClient();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var input = WaitForNode(component, "#delete-entity input");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            });

            var confirm = component.Find("#delete-entity button.btn-danger");

            await confirm.ClickAsync(new MouseEventArgs());

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

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            var input = WaitForNode(component, "#grantTypes input");

            await input.TriggerEventAsync("onfocus", new FocusEventArgs());

            var button = WaitForNode(component, "#grantTypes .dropdown-item.m-0.p-0.pl-1.pr-1");
            
            await button.ClickAsync(new MouseEventArgs());

            var idInput = component.Find("#id");
            Assert.NotNull(idInput);

            await idInput.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            });

            var nameInput = component.Find("#name");
            
            Assert.NotNull(nameInput);
            await nameInput.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            });

            button = WaitForNode(component, "#secrets button.btn-sm");

            await button.ClickAsync(new MouseEventArgs());

            var valueInput = component.Find("#secrets input[placeholder=value]");

            await valueInput.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            });

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync();

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
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId);

            var input = WaitForNode(component, "#urls input[name=\"cors\"]");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.Include(c => c.RedirectUris)
                    .FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                var uri = client?.RedirectUris.FirstOrDefault(u => (u.Kind & UriKinds.Cors) == UriKinds.Cors);
                Assert.NotNull(uri);
                Assert.Equal("HTTP://FILTERED:80", uri?.SanetizedCorsUri);
            });

            input = WaitForNode(component, "#urls input[name=\"cors\"]");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = false
            });

            form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var client = await context.Clients.Include(c => c.RedirectUris)
                    .FirstOrDefaultAsync(c => c.Id == clientId);
                Assert.NotNull(client);
                var uri = client?.RedirectUris.FirstOrDefault();
                Assert.NotNull(uri);
                Assert.Null(uri?.SanetizedCorsUri);
            });
        }

        [Fact]
        public async Task WhenWriter_should_be_able_to_clone_entity()
        {
            string clientId = await CreateClient("authorization_code");

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                clientId,
                true);

            var input = WaitForNode(component, "#name");

            Assert.Contains(input.Attributes, a => a.Value == $"Clone of {clientId}");
        }

        [Fact]
        public async Task SaveClik_should_validate_wsfed_entity()
        {
            var clientId = GenerateId();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            var idInput = WaitForNode(component, "#id");
            Assert.NotNull(idInput);

            await idInput.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            });

            var protocolTypeInputs = WaitForAllNodes(component, "input[type=radio]");
            Assert.NotNull(protocolTypeInputs);
            
            var protocolTypeInput = protocolTypeInputs[1];
            await protocolTypeInput.ChangeAsync(new ChangeEventArgs
            {
                Value = 1
            });

            var redirectUriInput = WaitForNode(component, "#redirect-uri");
            Assert.NotNull(redirectUriInput);

            await redirectUriInput
                .ChangeAsync(new ChangeEventArgs 
                { 
                    Value = clientId 
                })
                ;

            redirectUriInput = WaitForNode(component, "#redirect-uri");
            Assert.NotNull(redirectUriInput);

            await redirectUriInput
                .ChangeAsync(new ChangeEventArgs
                {
                    Value = clientId
                })
                ;

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync();

            Assert.Contains("The id must be an URI when protocol is WS-Federation. (ex: urn:wsfed).", component.Markup);
            Assert.Contains("The relying party is required.", component.Markup);
            Assert.Contains($"The url '{clientId}' is not valid.", component.Markup);
        }

        [Fact]
        public async Task SaveClick_should_validate_saml2p_entity()
        {
            var clientId = GenerateId();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            var idInput = WaitForNode(component, "#id");
            Assert.NotNull(idInput);

            await idInput.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            });

            var protocolTypeInputs = WaitForAllNodes(component, "input[type=radio]");
            Assert.NotNull(protocolTypeInputs);

            var protocolTypeInput = protocolTypeInputs[2];
            await protocolTypeInput.ChangeAsync(new ChangeEventArgs
            {
                Value = 2
            });

            var addButton = WaitForNode(component, "#urls button");
            Assert.NotNull(addButton);

            await addButton.ClickAsync(new MouseEventArgs());
            
            var urlInputs = WaitForAllNodes(component, "#urls input.form-control");

            await urlInputs[0].ChangeAsync(new ChangeEventArgs
            {
                Value = "https://test"
            });

            var urlCheckBoxes = WaitForAllNodes(component, "#urls input.form-check-input");
            await urlCheckBoxes[0].ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });

            addButton = WaitForNode(component, "#urls button");
            await addButton.ClickAsync(new MouseEventArgs());

            urlInputs = WaitForAllNodes(component, "#urls input.form-control");
            await urlInputs[1].ChangeAsync(new ChangeEventArgs
            {
                Value = "https://test"
            });


            urlCheckBoxes = WaitForAllNodes(component, "#urls input.form-check-input");
            await urlCheckBoxes[3].ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync();

            Assert.Contains("Uri must be unique.", component.Markup);
            Assert.Contains("Cannot have more than one URI per kind.", component.Markup);
            Assert.Contains($"Either a metadata URI or a X509Certificate secret must be set.", component.Markup);

            urlCheckBoxes = WaitForAllNodes(component, "#urls input.form-check-input");
            await urlCheckBoxes[4].ChangeAsync(new ChangeEventArgs
            {
                Value = true
            });

            form = component.Find("form");

            await form.SubmitAsync();

            Assert.Contains($"Either a metadata URI or a X509Certificate secret must be set.", component.Markup);
        }

        private async Task<string> CreateClient(string grantType = "hybrid", bool allowOfflineAccess = false)
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
                    AllowOfflineAccess = allowOfflineAccess,
                    RedirectUris = new List<ClientUri>
                    {
                        new ClientUri{ Id = GenerateId(), Uri = "http://filtered", Kind = UriKinds.Redirect },
                        new ClientUri{ Id = GenerateId(), Uri = "http://filtered/filtered", Kind = UriKinds.Cors | UriKinds.PostLogout }
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
