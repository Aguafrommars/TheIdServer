// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
                .AddConfigurationEntityFrameworkStores(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var sut = provider.GetRequiredService<ICorsPolicyService>();

            Assert.False(await sut.IsOriginAllowedAsync("http://test"));
        }
    }
}
