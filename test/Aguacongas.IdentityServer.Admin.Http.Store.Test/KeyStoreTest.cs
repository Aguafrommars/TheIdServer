// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Http.Store.Test
{
    public class KeyStoreTest
    {
        [Fact]
        public async Task GetAsync_should_not_be_implemented()
        {
            var sut = new KeyStore<Key>(Task.FromResult(new HttpClient()), new NullLogger<KeyStore<Key>>());

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(null));
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(null, null));
        }
    }
}
