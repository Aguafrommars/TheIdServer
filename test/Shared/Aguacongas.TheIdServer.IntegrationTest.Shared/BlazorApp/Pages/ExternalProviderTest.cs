// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.ExternalProvider;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ExternalProviderTest : EntityPageTestBase<page>
    {
        public ExternalProviderTest(TheIdServerFactory factory) : base(factory)
        {
        }

        public override string Entity => "externalprovider";

        [Fact]
        public async Task SaveClick_should_create_provider()
        {
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            WaitForNode(component, "option");

            var input = WaitForNode(component, "#name");

            var providerName = GenerateId();

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = providerName
            }).ConfigureAwait(false);

            input = WaitForNode(component, "#displayName");

            var displayName = GenerateId();

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = displayName
            }).ConfigureAwait(false);

            input = WaitForNode(component, "#type");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = "Google"
            }).ConfigureAwait(false);

            input = WaitForNode(component, "#clientId");

            var clientId = GenerateId();

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = clientId
            }).ConfigureAwait(false);

            input = WaitForNode(component, "#clientSecret");

            var clientSecret = GenerateId();

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = clientSecret
            }).ConfigureAwait(false);

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var provider = await context.Providers.FirstOrDefaultAsync(r => r.Id == providerName);
                Assert.NotNull(provider);
            });
        }


        [Fact]
        public async Task SaveClick_should_update_provider()
        {
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                "Google");

            var input = WaitForNode(component, "#displayName");

            var displayName = GenerateId();

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = displayName
            }).ConfigureAwait(false);

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var provider = await context.Providers.FirstOrDefaultAsync(r => r.DisplayName == displayName);
                Assert.NotNull(provider);
            });
        }

        [Fact]
        public async Task DeleteButtonClick_should_delete_Role()
        {
            string providerId = await CreateProvider();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                providerId);

            var input = WaitForNode(component, "#delete-entity input");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = providerId
            }).ConfigureAwait(false);

            var confirm = component.Find("#delete-entity button.btn-danger");

            await confirm.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var provider = await context.Providers.FirstOrDefaultAsync(p => p.Id == providerId);
                Assert.Null(provider);
            });
        }

        [Fact]
        public async Task ClickTransformationButtons_should_not_throw()
        {
            var providerId = await CreateProvider();
            var component = CreateComponent("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         providerId);

            var buttons = WaitForAllNodes(component, "#transformations button");

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            var expected = buttons.Count;
            await buttons.First().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var from = WaitForNode(component, "#fromClaimType");
            await from.ChangeAsync(new ChangeEventArgs
            {
                Value = "test"
            }).ConfigureAwait(false);

            var to = WaitForNode(component, "#toClaimType");
            await to.ChangeAsync(new ChangeEventArgs
            {
                Value = "test"
            }).ConfigureAwait(false);

            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            buttons = component.FindAll("#transformations button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await buttons.Last().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            buttons = component.FindAll("#transformations button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        [Fact]
        public async Task AddRemoveScope_should_not_throw()
        {
            var providerId = await CreateProvider();
            var component = CreateComponent("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         providerId);

            var input = WaitForNode(component, "#scope input.new-claim");

            await input.ChangeAsync(new ChangeEventArgs { Value = "name" }).ConfigureAwait(false);

            var divs = component.FindAll("ul.list-inline div.input-group-append.select");

            Assert.NotEmpty(divs);

            await divs.Last().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);
        }

        [Fact]
        public async Task RequiredHttpsMetadata_click_should_revalidate_MetadataAddress()
        {
            var providerId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(async c =>
            {
                await c.Providers.AddAsync(new ExternalProvider
                {
                    Id = providerId,
                    DisplayName = GenerateId(),
                    SerializedOptions = "{\"RemoteSignOutPath\":\"/signin-wsfed\",\"AllowUnsolicitedLogins\":false,\"RequireHttpsMetadata\":false,\"UseTokenLifetime\":true,\"Wtrealm\":\"urn:aspnetcorerp\",\"SignOutWreply\":null,\"Wreply\":null,\"SkipUnrecognizedRequests\":false,\"RefreshOnIssuerKeyNotFound\":true,\"MetadataAddress\":\"http://localhost:5001/wsfederation\",\"SignOutScheme\":null,\"SaveTokens\":false}",
                    SerializedHandlerType = "{\"Name\":\"Microsoft.AspNetCore.Authentication.WsFederation.WsFederationHandler\"}"
                });

                await c.SaveChangesAsync();
            });


            var component = CreateComponent("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         providerId);

            var input = WaitForNode(component, "#require-https input");

            await input.ChangeAsync(new ChangeEventArgs { Value = true }).ConfigureAwait(false);

            Assert.NotNull(component.Find("li.validation-message"));

            input = WaitForNode(component, "#require-https input");

            await input.ChangeAsync(new ChangeEventArgs { Value = false }).ConfigureAwait(false);

            Assert.Throws<ElementNotFoundException>(() => component.Find("li.validation-message"));
        }

        private async Task<string> CreateProvider()
        {
            var providerId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(async c =>
            {
                await c.Providers.AddAsync(new ExternalProvider
                {
                    Id = providerId,
                    DisplayName = GenerateId(),
                    SerializedOptions = "{\"ClientId\":\"818322595124 - h0nd8080luc71ba2i19a5kigackfm8me.apps.googleusercontent.com\",\"ClientSecret\":\"ac_tx - O9XvZGNRi4HYfPerx2\",\"AuthorizationEndpoint\":\"https://accounts.google.com/o/oauth2/v2/auth\",\"TokenEndpoint\":\"https://www.googleapis.com/oauth2/v4/token\",\"UserInformationEndpoint\":\"https://www.googleapis.com/oauth2/v2/userinfo\",\"Events\":{},\"ClaimActions\":[{\"JsonKey\":\"id\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"given_name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"family_name\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"link\",\"ClaimType\":\"urn:google:profile\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"},{\"JsonKey\":\"email\",\"ClaimType\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress\",\"ValueType\":\"http://www.w3.org/2001/XMLSchema#string\"}],\"Scope\":[\"openid\",\"profile\",\"email\"],\"BackchannelTimeout\":\"00:01:00\",\"Backchannel\":{\"DefaultRequestHeaders\":[{\"Key\":\"User-Agent\",\"Value\":[\"Microsoft\",\"ASP.NET\",\"Core\",\"OAuth\",\"handler\"]}],\"DefaultRequestVersion\":\"1.1\",\"Timeout\":\"00:01:00\",\"MaxResponseContentBufferSize\":10485760},\"CallbackPath\":\"/signin-google\",\"ReturnUrlParameter\":\"ReturnUrl\",\"SignInScheme\":\"Identity.External\",\"RemoteAuthenticationTimeout\":\"00:15:00\",\"CorrelationCookie\":{\"Name\":\".AspNetCore.Correlation.\",\"HttpOnly\":true,\"IsEssential\":true}}",
                    SerializedHandlerType = "{\"Name\":\"Microsoft.AspNetCore.Authentication.Google.GoogleHandler\"}"
                });

                await c.SaveChangesAsync();
            });

            return providerId;
        }
    }
}
