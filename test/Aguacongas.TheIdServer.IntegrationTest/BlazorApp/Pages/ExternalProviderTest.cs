// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ExternalProviderTest : EntityPageTestBase
    {
        public ExternalProviderTest(ApiFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        public override string Entity => "externalprovider";

        [Fact]
        public async Task SaveClick_should_create_provider()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForNode(host, component, "option");

            var input = WaitForNode(host, component, "#name");

            var providerName = GenerateId();

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(providerName));

            input = WaitForNode(host, component, "#displayName");

            var displayName = GenerateId();

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(displayName));

            input = WaitForNode(host, component, "#type");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync("Google"));

            input = WaitForNode(host, component, "#clientId");

            var clientId = GenerateId();

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(clientId));

            input = WaitForNode(host, component, "#clientSecret");

            var clientSecret = GenerateId();

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(clientSecret));

            var form = component.Find("form");
            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var provider = await context.Providers.FirstOrDefaultAsync(r => r.Scheme == providerName);
                Assert.NotNull(provider);
            });
        }


        [Fact]
        public async Task SaveClick_should_update_provider()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                "Google",
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#displayName");

            var displayName = GenerateId();

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(displayName));

            var form = component.Find("form");
            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

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

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                providerId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#delete-entity input");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(providerId));

            var confirm = component.Find("#delete-entity button.btn-danger");

            await host.WaitForNextRenderAsync(() => confirm.ClickAsync());

            WaitForDeletedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var provider = await context.Providers.FirstOrDefaultAsync(p => p.Scheme == providerId);
                Assert.Null(provider);
            });
        }

        [Fact]
        public async Task ClickTransformationButtons_should_not_throw()
        {
            var apiId = await CreateProvider();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var buttons = WaitForAllNodes(host, component, "#transformations button");

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            var expected = buttons.Count;
            await host.WaitForNextRenderAsync(() => buttons.First().ClickAsync());

            var from = host.WaitForNode(component, "#fromClaimType");
            await host.WaitForNextRenderAsync(() => from.ChangeAsync("test"));

            var to = host.WaitForNode(component, "#toClaimType");
            await host.WaitForNextRenderAsync(() => to.ChangeAsync("test"));

            var form = component.Find("form");
            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            buttons = component.FindAll("#transformations button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await host.WaitForNextRenderAsync(() => buttons.Last().ClickAsync());

            buttons = component.FindAll("#transformations button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        private async Task<string> CreateProvider()
        {
            var providerId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(async c =>
            {
                await c.Providers.AddAsync(new SchemeDefinition
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
