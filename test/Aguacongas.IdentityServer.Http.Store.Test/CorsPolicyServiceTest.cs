using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class CorsPolicyServiceTest
    {
        [Fact]
        public async Task IsOriginAllowedAsync_should_call_store_GetAsync()
        {
            var storeMock = new Mock<IAdminStore<ClientUri>>();
            var sut = new CorsPolicyService(storeMock.Object);

            storeMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ClientUri>
                {
                    Items = new List<ClientUri>(0)
                })
                .Verifiable();

            await sut.IsOriginAllowedAsync("http://test");

            storeMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => p.Filter == $"{nameof(ClientUri.SanetizedCorsUri)} eq 'HTTP://TEST:80'"), default));
        }
    }
}
