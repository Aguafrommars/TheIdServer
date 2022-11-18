using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Test.Shared.Store
{
    public class ResourceStoreTest
    {
        [Fact]
        public void Constructor_should_check_arguments()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceStore(null, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new ResourceStore(new Mock<IAdminStore<ProtectResource>>().Object, null, null, null));
            Assert.Throws<ArgumentNullException>(() => new ResourceStore(new Mock<IAdminStore<ProtectResource>>().Object,
                new Mock<IAdminStore<IdentityResource>>().Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new ResourceStore(new Mock<IAdminStore<ProtectResource>>().Object, 
                new Mock<IAdminStore<IdentityResource>>().Object, new Mock<IAdminStore<ApiScope>>().Object, null));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_should_return_empty_when_not_relation_found()
        {
            var storeMock = new Mock<IAdminStore<ApiApiScope>>();
            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<ApiApiScope>
            {
                Items = Array.Empty<ApiApiScope>(),
                Count = 0
            });

            var sut = new ResourceStore(new Mock<IAdminStore<ProtectResource>>().Object,
                new Mock<IAdminStore<IdentityResource>>().Object,
                new Mock<IAdminStore<ApiScope>>().Object,
                storeMock.Object);

            var result = await sut.FindApiResourcesByScopeNameAsync(new[] { Guid.NewGuid().ToString() }).ConfigureAwait(false);

            Assert.Empty(result);
        }
    }
}
