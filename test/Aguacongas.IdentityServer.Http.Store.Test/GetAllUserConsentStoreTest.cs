using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class GetAllUserConsentStoreTest
    {
        [Fact]
        public async Task GetAllUserConsent_should_call_store_GetAsync()
        {
            var storeMock = new Mock<IAdminStore<UserConsent>>();
            var serializerMock = new Mock<IPersistentGrantSerializer>();
            var sut = new GetAllUserConsentStore(storeMock.Object, serializerMock.Object);

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<UserConsent>
                {
                    Count = 1,
                    Items = new List<UserConsent>
                    {
                        new UserConsent()
                    }
                })
                .Verifiable();

            await sut.GetAllUserConsent("test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == "UserId eq 'test'"), default));
        }
    }
}
