// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Services
{
    public class RoleAdminStoreTest
    {
        [Fact]
        public void Construtor_should_check_dependencies()
        {
            Assert.Throws<ArgumentNullException>(() => new RoleAdminStore(null));
        }

        [Fact]
        public async Task GetAsync_page_should_not_be_implememted()
        {
            var storeMock = new Mock<IAdminStore<Role>>();
            var sut = new RoleAdminStore(storeMock.Object);
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(new PageRequest()));
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(new PageRequest(), CancellationToken.None));
        }

        [Fact]
        public async Task UpdateAsync_should_not_be_implememted()
        {
            var storeMock = new Mock<IAdminStore<Role>>();
            var sut = new RoleAdminStore(storeMock.Object);
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new Role()));
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new Role(), CancellationToken.None));
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new object()));
            await Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new object(), CancellationToken.None));
        }
    }
}
