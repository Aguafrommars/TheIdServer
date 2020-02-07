using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Http.Store.Test
{
    public class ResourceStoreTest
    {
        [Fact]
        public async Task GetAllResourcesAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock, out ResourceStore sut);

            await sut.GetAllResourcesAsync().ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => 
                p.Expand == "ApiClaims,ApiScopeClaims,Secrets,Scopes,Properties"), default));
            identityStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => 
                p.Expand == "IdentityClaims,Properties"), default));
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock, out ResourceStore sut);

            await sut.FindIdentityResourcesByScopeAsync(new string[] { "test" }).ConfigureAwait(false);

            identityStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == "Id eq 'test'"), default));
        }

        [Fact]
        public async Task FindApiResourcesByScopeAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock, out ResourceStore sut);

            await sut.FindApiResourcesByScopeAsync(new string[] { "test" }).ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == "Scopes/any(s:s/Scope eq 'test')"), default));
        }

        [Fact]
        public async Task FindApiResourceAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock, out ResourceStore sut);

            apiStoreMock.Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<GetRequest>(), default))
                .Verifiable();

            await sut.FindApiResourceAsync("test").ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync("test", It.Is<GetRequest>(p =>
                p.Expand == "ApiClaims,ApiScopeClaims,Secrets,Scopes,Properties"), default));
        }

        private static void CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock, out Mock<IAdminStore<IdentityResource>> identityStoreMock, out ResourceStore sut)
        {
            apiStoreMock = new Mock<IAdminStore<ProtectResource>>();
            identityStoreMock = new Mock<IAdminStore<IdentityResource>>();
            sut = new ResourceStore(apiStoreMock.Object, identityStoreMock.Object);

            apiStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ProtectResource>
                {
                    Items = new List<ProtectResource>(0)
                }).Verifiable();

            identityStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<IdentityResource>
                {
                    Items = new List<IdentityResource>(0)
                }).Verifiable();
        }
    }
}
