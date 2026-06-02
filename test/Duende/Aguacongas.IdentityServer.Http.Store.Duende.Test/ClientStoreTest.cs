// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test;

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
                RedirectUris = [],
                AllowedGrantTypes = [],
                AllowedScopes = [],
                ClientClaims = [],
                ClientSecrets = [],
                IdentityProviderRestrictions = [],
                Properties = [],
                Resources = [],
                AllowedIdentityTokenSigningAlgorithms = []
            })
            .Verifiable();

        await sut.FindClientByIdAsync("test", default);

        storeMock.Verify(m => m.GetAsync(It.Is<string>(id => id == "test"), It.IsAny<GetRequest>(), default));
    }
}
