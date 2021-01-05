// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Moq;
using System;
using System.Threading;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Services
{
    public class UserAdminStoreTest
    {
        [Fact]
        public void Construtor_should_check_dependencies()
        {
            Assert.Throws<ArgumentNullException>(() => new UserAdminStore(null));
        }

        [Fact]
        public void GetAsync_page_should_not_be_implememted()
        {
            var storeMock = new Mock<IAdminStore<User>>();
            var sut = new UserAdminStore(storeMock.Object);
            Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(new PageRequest()));
            Assert.ThrowsAsync<NotImplementedException>(() => sut.GetAsync(new PageRequest(), CancellationToken.None));
        }
    }
}
