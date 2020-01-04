using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.IntegrationTest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Pages
{
    [Collection("api collection")]
    public class ApiTest
    {
        private readonly ApiFixture _fixture;

        public ApiTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _fixture.TestOutputHelper = testOutputHelper;
        }


        [Fact]
        public void WhenNonWriter_should_disable_inputs()
        {
            CreateTestHost("Bob Smith",
                AuthorizationOptionsExtensions.READER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender(() => Thread.Sleep(500));

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.All(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public void WhenWriter_should_enable_inputs()
        {
            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender();

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.DoesNotContain(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_properties_scopes_scopeClaims_and_secret()
        {
            var apiId = GenerateId();
            await _fixture.DbActionAsync<IdentityServerDbContext>(context =>
            {
                context.Apis.Add(new ProtectResource
                {
                    Id = apiId,
                    DisplayName = apiId,
                    ApiClaims = new List<ApiClaim>
                    {
                        new ApiClaim { Id = GenerateId(), Type = "filtered" }
                    },
                    Properties = new List<ApiProperty>
                    {
                        new ApiProperty { Id = GenerateId(), Key = "filtered", Value = "filtered" }
                    },
                    Scopes = new List<ApiScope>
                    {
                       new ApiScope
                       {
                           Id = GenerateId(),
                           Scope = apiId,
                           DisplayName = "test",
                           ApiScopeClaims = new List<ApiScopeClaim>
                           {
                               new ApiScopeClaim { Id = GenerateId(), Type = "filtered" }
                           }
                       },
                       new ApiScope
                       {
                           Id = GenerateId(),
                           Scope = "filtered",
                           DisplayName = "filtered",
                           ApiScopeClaims = new List<ApiScopeClaim>()
                       }
                    },
                    Secrets = new List<ApiSecret>
                    {
                        new ApiSecret { Id = GenerateId(), Type="SHA256", Value = "filtered" }
                    }
                });

                return context.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender();

            var markup = component.GetMarkup();

            Assert.Contains("Loading...", markup);

            host.WaitForNextRender();

            markup = component.GetMarkup();

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(() => filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = apiId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        private void CreateTestHost(string userName,
            string role,
            string id,
            out TestHost host,
            out RenderedComponent<App> component,
            out MockHttpMessageHandler mockHttp)
        {
            TestUtils.CreateTestHost(userName,
                new List<SerializableClaim>
                {
                    new SerializableClaim
                    {
                        Type = "role",
                        Value = AuthorizationOptionsExtensions.READER
                    },
                    new SerializableClaim
                    {
                        Type = "role",
                        Value = role
                    }
                },
                $"http://exemple.com/protectresource/{id}", 
                _fixture.Sut,
                _fixture.TestOutputHelper,
                out host,
                out component,
                out mockHttp);
        }

        private static string GenerateId() => Guid.NewGuid().ToString();
    }
}
