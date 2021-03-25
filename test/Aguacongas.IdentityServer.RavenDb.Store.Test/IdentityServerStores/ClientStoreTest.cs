// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.IdentityServerStores
{
    public class ClientStoreTest
    {
        [Fact]
        public void Construrctor_should_check_paameter()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientStore(null));
        }

        [Fact]
        public async Task FindClientByIdAsync_should_return_client()
        {
            using var store = new RavenDbTestDriverWrapper().GetDocumentStore();
            using var s1 = store.OpenAsyncSession();

            await s1.StoreAsync(new Entity.Client
            {
                Id = "test",
                AllowedGrantTypes = new List<Entity.ClientGrantType>
                {
                    new Entity.ClientGrantType
                    {
                        Id = $"{nameof(Entity.ClientGrantType)}/test"
                    }
                }
            }, $"{nameof(Entity.Client)}/test");
            await s1.StoreAsync(new Entity.ClientGrantType
            {
                Id = "test",
                GrantType = "client_credential"
            }, $"{nameof(Entity.ClientGrantType)}/test");
            await s1.SaveChangesAsync();
            
            var sut = new ClientStore(new ScopedAsynDocumentcSession(store.OpenAsyncSession()));

            var result = await sut.FindClientByIdAsync("test");

            Assert.NotNull(result);
            Assert.Contains(result.AllowedGrantTypes, g => g == "client_credential");
        }
    }
}
