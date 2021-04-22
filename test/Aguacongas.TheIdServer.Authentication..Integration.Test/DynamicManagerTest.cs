// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.AspNetCore.Authentication.TestBase;
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.Authentication.IntegrationTest
{
    public class DynamicManagerTest : DynamicManagerTestBase<SchemeDefinition>, IClassFixture<TheIdServerTestFixture>
    {
        private readonly TheIdServerTestFixture _fixture;

        public DynamicManagerTest(ITestOutputHelper output, TheIdServerTestFixture fixture) : base(output)
        {
            _fixture = fixture;
        }

        protected override DynamicAuthenticationBuilder AddStore(DynamicAuthenticationBuilder builder)
        {
            var httpClient = _fixture.Sut.CreateClient();
            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "/api");
            builder.Services.AddIdentityServer4AdminHttpStores(p =>
            {
                return Task.FromResult(httpClient);
            });

            _fixture.Sut.Services.GetRequiredService<TestUserService>().SetTestUser(true,
                new Claim[]
                {
                    new Claim("role", SharedConstants.WRITER),
                    new Claim("role", SharedConstants.READER)
                });

            return builder.AddTheIdServerStore();
        }
    }
}
