// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;
using ISConfiguration = Duende.IdentityServer.Configuration;

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
                .Configure<ISConfiguration.IdentityServerOptions>(options => options.Caching.CorsExpiration = TimeSpan.FromMinutes(1))
                .AddTransient(p => p.GetRequiredService<IOptions<ISConfiguration.IdentityServerOptions>>().Value)
                .AddConfigurationStores()
                .AddConfigurationEntityFrameworkStores(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<ICorsPolicyService>();

            Assert.False(await sut.IsOriginAllowedAsync("http://test"));
        }
    }
}
