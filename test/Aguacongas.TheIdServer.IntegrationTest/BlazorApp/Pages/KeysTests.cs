﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class KeysTests
    {
        private TestHost _host;
        public ApiFixture Fixture { get; }

        public KeysTests(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task DataProtectionRevokeClick_should_revoke_key()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out RenderedComponent<App> component,
                out TestHost host);

            var keyId = host.WaitForNode(component, "#data-protection-keys div.modal.fade").Attributes.First(a => a.Name == "id").Value.Substring("revoke-entity-".Length);

            var input = host.WaitForNode(component, $"#revoke-entity-{keyId} input");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(keyId));

            var confirm = component.Find($"#revoke-entity-{keyId} .modal-footer button.btn-danger");

            await host.WaitForNextRenderAsync(() => confirm.ClickAsync());

            WaitForToast("Revoked", host, component);

            await Fixture.DbActionAsync<OperationalDbContext>(async context =>
            {
                var revoked = await context.DataProtectionKeys.FirstOrDefaultAsync(k => k.FriendlyName == $"revocation-{keyId}");
                Assert.NotNull(revoked);
            });
        }

        [Fact]
        public async Task SigningRevokeClikc_should_revoke_key()
        {
            await Fixture.DbActionAsync<OperationalDbContext>(async context =>
            {
                var id = Guid.NewGuid();
                await context.KeyRotationKeys.AddAsync(new KeyRotationKey
                {
                    FriendlyName = $"key-{id}",
                    Xml = @$"<key id=""{id}"" version=""1"">
    <creationDate>2020-10-08T07:44:21.391891Z</creationDate>
    <activationDate>2020-10-08T07:44:21.3792973Z</activationDate>
    <expirationDate>2020-10-22T07:44:21.3792973Z</expirationDate>
    <descriptor deserializerType=""Aguacongas.IdentityServer.KeysRotation.RsaEncryptorDescriptorDeserializer, Aguacongas.IdentityServer.KeysRotation, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null"">
        <descriptor>
            <encryption algorithm=""System.Security.Cryptography.RSA, System.Security.Cryptography.Algorithms, Version = 4.3.2.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a"" keyLength=""2048""/>
            <keyId>74A71DC1642A06B8A753E8FCA13EC40A</keyId>
            <key p4:requiresEncryption=""true"" xmlns:p4=""http://schemas.asp.net/2015/03/dataProtection"">
                <value>eyJEIjoicFBCU0tPWUdlaTBFcUVmTE5jOG43ZHh4eFluS3Z4dTlEUWE3WTdQVy96bEZQSnBFUEh3MWpKNGVsaEl2KzBJNWNaUDVUMHFpLzdnWm5tZWs4Z2JTbUFUampLNUo1VnA2dWJ5VWFrY25oQ2NBcWxYbERjTkI3U21jMmoyQjUzSWhhYWE5d1U1OUs0NnJhRjhKejIxaHp4dTkwaFNPNUlBcm03enk4TEVoZTdVNlUxWm5zclBWZHpVUWtsNUVqUmt3eDF6dE1yNk5scU13V2RDb2N4Rmt3ZTQ1N1ZjV0JqRDBPODZ5Q0lJdDJVRXdoQkpTUGRjREJOa3lmMzlhZEZHN0V6My9HUkRuS0I4Z2diZHFVRW1zSkl6QmkyYmM5dERVYjArbzVwVThsUStsd0RoSFBsR1dSTUhOdTFPV0JYODYxWGhGY2lINm8xS3ZueGRXNE5JNkZRPT0iLCJEUCI6InFQWGpoQlJZRWdkbkVaNUtYcndobTE2cm9LSkRRNm0wcXByaU1qK29aeTV6V3pBWWVqeHd4SFNCOW1JakxiM0VqRVBJUnJkeFljM0xYd24vRy92aStwa25yb2YwakhTT3Jkc2dKdEx1SEs1MzU3U3hlMm1PU3JpWDdWNUgxN1hydkQvKzk4RGtEUEQxa1dNSXF0cmxWbFNSb0xEQzM3Q1JjTHNOckZyOVgzRT0iLCJEUSI6Ik5Ia1pTNWphYUVISGcwekFSU21mRkhlZEV2REJWenM4U3N5Q1p6LzQ4Rk5ObGVidkVIZ1JsK2plWUtiNjZHOXl0Q0UzYnFsUGNERzIyVVdXNmRRcFdkNXUwOWRmYnNpVUZocmlMajh4WlNNdUhhOFBYemZ0OFdraEpYNmZ1NTZCbmtoQUNxeGppZTVaZlpnOUNJS1BUTGVQVjg5R0J4TnhTcFlTdEkzbmMvcz0iLCJFeHBvbmVudCI6IkFRQUIiLCJJbnZlcnNlUSI6IlZyOGlwV05TVUVqaFBuTVRtejR4L204dlE0Tm5oSEVuU3Z3ak9PQ3lsVjM5THJRTiszSXo0dWIxU054Y21KWktVdWJnOXg4S1pKL2ZobHV1MVRvdElJNzBiMEp0UGoraEtFY3c0ODdsK3RlY2NSQVhJVWE4bkZLMXZOVHlCZnlUUUlGQk4wQS95TEpodWUwdGEydFNsSGNjYkFxdzl1OUZqeDJXclVqOWJjdz0iLCJNb2R1bHVzIjoiNGdPd1FKNHRiR2R0aWFmblhYYlNUMkdsWG9mR3RkZUxzOGNIWlBEbmE4eEdVblc2Nlk4UFRyMmVLbTNXRGNvYVVyb2dVTElseXB1c0ZaRXZ2UUZiaDI5SXZBbHZOYWJBTVVpRFNIZUY1R2VXbitzRHNtTjdjeWpGWWZNWjE1djVpR21YNmNGNzRFVE96SVAyNzJXdUF0aGVhSjBkanJrRStOdHpWMy92SEpQL3RoTHM5S0RoT203RUtaVmhDdnlkalNNVnpkbTZQTkV3ekF4UHRVSXRqeEN0SzBwYkU1SXhMamJzT0FScEpnQ3lJLytOWlRlcGVUYUFXU05TNmd2TWUyTS9yREJFMm05bTNPdjNFNGtTNjdxU2FqZDJIa29TUkpxZWRaRk4xQkY0MDUwU1NRUTkrWkZUUkkzV3llSUcwd2VDeFpnakhyaFl0WUZObDNaVUhRPT0iLCJQIjoiNGtkTGdqei9mVUdMTTB1RThaaDJwTThjLzdJdHBldjBqalMzQ2hoL0dCbVZmN1JGQmhnVWJZSTdFUGlQQzI0ZThDNmxNb01lVG14MlFOQVNSelFaT2x5QWNCTHV2U0RwTkQwczFRa2JmbW1ucE5UbWJncURidVh3NFp0cklJR1dZV2ZqMVUvSmY4dkltaU9LdVQvU3VWMit2Wmg2OVpqaTFrR3ljcUhZWVZjPSIsIlEiOiIvN09EY3ZSVTZxcGJsQW9LejJialdxMGg2V0loVDQ3RmdhbHFDZWZBejFVYWJKOTBXVXVNOCt2eFZZTHV1V2dNR1IvZ0JPTVdna0l2WUtYODFFSmd4aHhnRGlDd0p0VFRrUXVQUWtsYWhHRWp5SERJWGNwaHdDT3VsS2h4cEkrZlpOMEFROWsyQWcyN2IzR3JUeDdHbUptVEVnbXFwMzh0MFVhOU4vU0F5YXM9In0=</value>
            </key>
        </descriptor >
    </descriptor>
</key>"
                });
                await context.SaveChangesAsync();
            });


            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                out RenderedComponent<App> component,
                out TestHost host);

            var keyId = host.WaitForNode(component, "#signing-keys div.modal.fade").Attributes.First(a => a.Name == "id").Value.Substring("revoke-entity-".Length);

            var input = host.WaitForNode(component, $"#revoke-entity-{keyId} input");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(keyId));

            var confirm = component.Find($"#revoke-entity-{keyId} .modal-footer button.btn-danger");

            await host.WaitForNextRenderAsync(() => confirm.ClickAsync());

            WaitForToast("Revoked", host, component);

            await Fixture.DbActionAsync<OperationalDbContext>(async context =>
            {
                var revoked = await context.KeyRotationKeys.FirstOrDefaultAsync(k => k.FriendlyName == $"revocation-{keyId}");
                Assert.NotNull(revoked);
            });
        }

        protected static void WaitForToast(string text, TestHost host, RenderedComponent<App> component)
        {
            var toasts = component.FindAll(".toast-body.text-success");
            while (!toasts.Any(t => t.InnerText.Contains(text)))
            {
                host.WaitForNextRender();
                toasts = component.FindAll(".toast-body.text-success");
            }
        }


        private void CreateTestHost(string userName,
           string role,
           out RenderedComponent<App> component,
           out TestHost host)
        {
            TestUtils.CreateTestHost(userName,
                new Claim[]
                {
                    new Claim("role", SharedConstants.READER),
                    new Claim("role", role)
                },
                $"http://exemple.com/keys",
                Fixture.Sut,
                Fixture.TestOutputHelper,
                out host,
                out component,
                out MockHttpMessageHandler _,
                true);
            _host = host;
        }
    }
}
