// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using RelyingPartyPage = Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParty.RelyingParty;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class RelyingPartyTest : EntityPageTestBase<RelyingPartyPage>
    {
        public override string Entity => "relyingparty";
        public RelyingPartyTest(TheIdServerFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task OnInitializedAsync_should_get_certificate_thumbprint()
        {
            string relyingPartyId = await CreateEntity(await File.ReadAllBytesAsync("test.crt").ConfigureAwait(false));

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId);

            Assert.Contains("A certificate chain processed", component.Markup);
        }

        [Fact]
        public async Task SetCertificateAsync_should_get_certificate_thumbprint()
        {
            string relyingPartyId = await CreateEntity(null);

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId);

            var inputFile = component.FindComponent<InputFile>();
            await component.InvokeAsync(()=> inputFile.Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs(new List<IBrowserFile>
            {
                new FakeBrowserFile()
            })).ConfigureAwait(false)).ConfigureAwait(false);

            DotNetDispatcher.BeginInvokeDotNet(new JSRuntimeImpl(), new DotNetInvocationInfo(null, "NotifyChange", 1, default), "[[{ \"name\": \"test.crt\" }]]");

            component.WaitForState(() => component.Markup.Contains("Invalid file"));

            Assert.Contains("Invalid file", component.Markup);
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_claims()
        {
            string relyingPartyId = await CreateEntity(null);

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = relyingPartyId
            }).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task SaveClick_should_update_entity()
        {
            string relyingPartyId = await CreateEntity(null);

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId);

            var input = component.Find("#description");

            Assert.NotNull(input);

            var expected = GenerateId();
            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = expected
            }).ConfigureAwait(false);

            Assert.Contains(expected, component.Markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var relyingParty = await context.RelyingParties.FirstOrDefaultAsync(a => a.Id == relyingPartyId);
                Assert.Equal(expected, relyingParty?.Description);
            });
        }

        [Fact]
        public async Task WhenWriter_should_be_able_to_clone_entity()
        {
            string relyingPartyId = await CreateEntity(null);

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                relyingPartyId,
                true);

            var input = component.Find("#description");

            Assert.Contains(input.Attributes, a => a.Value == $"Clone of {relyingPartyId}");
        }

        private async Task<string> CreateEntity(byte[]? certificate)
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

        class FakeBrowserFile : IBrowserFile
        {
            public string Name => GenerateId();

            public DateTimeOffset LastModified => DateTimeOffset.Now;

            public long Size => 10;

            public string ContentType => "application/pdf";

            public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
            {
                return new MemoryStream(10);
            }
        }
    }
}
