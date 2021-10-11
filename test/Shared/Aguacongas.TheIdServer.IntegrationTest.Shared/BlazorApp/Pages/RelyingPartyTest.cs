// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class RelyingPartyTest : EntityPageTestBase
    {
        public override string Entity => "relyingparty";
        public RelyingPartyTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task OnInitializedAsync_should_get_certificate_thumbprint()
        {
            string relyingPartyId = await CreateEntity(await File.ReadAllBytesAsync("test.crt").ConfigureAwait(false));

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _);

            WaitForLoaded(host, component);

            WaitForContains(host, component, "A certificate chain processed");

            string markup = component.GetMarkup();

            Assert.Contains("A certificate chain processed", markup);
        }

        [Fact]
        public async Task SetCertificateAsync_should_get_certificate_thumbprint()
        {
            string relyingPartyId = await CreateEntity(null);

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler _,
                true);

            var jsRuntime = host.ServiceProvider.GetRequiredService<JSRuntimeImpl>();

            WaitForLoaded(host, component);

            WaitForContains(host, component, "filtered");

            jsRuntime.Called.WaitOne();

            await Task.Delay(100).ConfigureAwait(false);

            DotNetDispatcher.BeginInvokeDotNet(jsRuntime, new DotNetInvocationInfo(null, "NotifyChange", 1, default), "[[{ \"name\": \"test.crt\" }]]");

            host.WaitForNextRender();

            string markup = component.GetMarkup();

            Assert.Contains("Invalid file", markup);
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_claims()
        {
            string relyingPartyId = await CreateEntity(null);

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForContains(host, component, "filtered");

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await host.WaitForNextRenderAsync(() => filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = relyingPartyId
            }));

            string markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task SaveClick_should_update_entity()
        {
            string relyingPartyId = await CreateEntity(null);

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = component.Find("#description");

            Assert.NotNull(input);

            var expected = GenerateId();
            await host.WaitForNextRenderAsync(() => input.ChangeAsync(expected));

            var markup = component.GetMarkup();

            Assert.Contains(expected, markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var relyingParty = await context.RelyingParties.FirstOrDefaultAsync(a => a.Id == relyingPartyId);
                Assert.Equal(expected, relyingParty.Description);
            });
        }

        private async Task<string> CreateEntity(byte[] certificate)
        {
            var relyingPartyId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.RelyingParties.Add(new RelyingParty
                {
                    Id = relyingPartyId,
                    Description = relyingPartyId,
                    ClaimMappings = new List<RelyingPartyClaimMapping>
                    {
                        new RelyingPartyClaimMapping { Id = GenerateId(), FromClaimType = "filtered", ToClaimType = "urn:filtered" }
                    },
                    DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                    EncryptionCertificate = certificate,
                    SamlNameIdentifierFormat = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
                    SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                    TokenType = "urn:oasis:names:tc:SAML:1.0:assertion"
                });

                return context.SaveChangesAsync();
            });
            return relyingPartyId;
        }
    }
}
