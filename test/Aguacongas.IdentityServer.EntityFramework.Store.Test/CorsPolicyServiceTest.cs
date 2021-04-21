// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Test
{
    public class CorsPolicyServiceTest
    {
        [Fact]
        public void Constructor_should_validate_parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new CorsPolicyService(null));
        }

        [Fact]
        public async Task IsOriginAllowedAsync_should_return_false()
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .Configure<IdentityServer4.Configuration.IdentityServerOptions>(options => options.Caching.CorsExpiration = TimeSpan.FromMinutes(1))
                .AddTransient(p => p.GetRequiredService<IOptions<IdentityServer4.Configuration.IdentityServerOptions>>().Value)
                .AddConfigurationEntityFrameworkStores<SchemeDefinition>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<ICorsPolicyService>();

            Assert.False(await sut.IsOriginAllowedAsync("http://test"));
        }
    }
}
