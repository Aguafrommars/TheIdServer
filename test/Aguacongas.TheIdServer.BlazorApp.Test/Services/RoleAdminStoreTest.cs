using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Moq;
using System;
using System.Threading;
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
        public void GetAsync_page_should_not_be_implememted()
        {
            var storeMock = new Mock<IAdminStore<Role>>();
            var sut = new RoleAdminStore(storeMock.Object);
            Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(new PageRequest()));
            Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(new PageRequest(), CancellationToken.None));
        }

        [Fact]
        public void UpdateAsync_should_not_be_implememted()
        {
            var storeMock = new Mock<IAdminStore<Role>>();
            var sut = new RoleAdminStore(storeMock.Object);
            Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new Role()));
            Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new Role(), CancellationToken.None));
            Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new object()));
            Assert.ThrowsAsync<NotImplementedException>(() => sut.UpdateAsync(new object(), CancellationToken.None));
        }
    }
}
