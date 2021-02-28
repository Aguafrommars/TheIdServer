// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class CorsPolicyServiceTest
    {
        [Fact]
        public async Task IsOriginAllowedAsync_should_check_SanetizedCorsUri()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.ClientUri
            {
                Id = "allowed",
                Kind = Entity.UriKinds.Cors | Entity.UriKinds.Redirect,
                SanetizedCorsUri = "HTTP://ALLOWED:5000"
            }, $"{nameof(Entity.ClientUri)}/allowed");

            await s1.StoreAsync(new Entity.ClientUri
            {
                Id = "notallowed",
                Kind = Entity.UriKinds.Redirect,
                SanetizedCorsUri = "HTTP://NOTALLOWED:5000"
            }, $"{nameof(Entity.ClientUri)}/notallowed");
            await s1.SaveChangesAsync();

            var sut = new CorsPolicyService(store.OpenAsyncSession());

            Assert.True(await sut.IsOriginAllowedAsync("http://allowed:5000"));
            Assert.False(await sut.IsOriginAllowedAsync("http://notallowed:5000"));
        }
    }
}
