// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class ClientStoreTest
    {
        [Fact]
        public async Task FindClientByIdAsync_should_call_store_GetAsync()
        {
            var storeMock = new Mock<IAdminStore<Client>>();
            var sut = new ClientStore(storeMock.Object);

            storeMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .ReturnsAsync(new Client
                {
                    RedirectUris = new List<ClientUri>(0),
                    AllowedGrantTypes = new List<ClientGrantType>(0),
                    AllowedScopes = new List<ClientScope>(0),
                    ClientClaims = new List<ClientClaim>(0),
                    ClientSecrets = new List<ClientSecret>(0),
                    IdentityProviderRestrictions = new List<ClientIdpRestriction>(0),
                    Properties = new List<ClientProperty>(0),
                    Resources = new List<ClientLocalizedResource>(0),
                    AllowedIdentityTokenSigningAlgorithms = new List<ClientAllowedIdentityTokenSigningAlgorithm>(0)
                })
                .Verifiable();

            await sut.FindClientByIdAsync("test");

            storeMock.Verify(m => m.GetAsync(It.Is<string>(id => id == "test"), It.IsAny<GetRequest>(), default));
        }
    }
}
