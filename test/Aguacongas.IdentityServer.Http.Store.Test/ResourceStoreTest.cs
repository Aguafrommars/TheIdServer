// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
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
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out Mock<IAdminStore<ApiApiScope>> apiApiScopeStoreMock,
                out ResourceStore sut);

            await sut.GetAllResourcesAsync().ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => 
                p.Expand == $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"), default));
            identityStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p => 
                p.Expand == $"{nameof(IdentityResource.IdentityClaims)},{nameof(IdentityResource.Properties)},{nameof(IdentityResource.Resources)}"), default));
            apiScopeStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Expand == $"{nameof(ApiScope.ApiScopeClaims)},{nameof(ApiScope.Properties)},{nameof(ApiScope.Resources)}"), default));
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out Mock<IAdminStore<ApiApiScope>> apiApiScopeStoreMock,
                out ResourceStore sut);

            await sut.FindIdentityResourcesByScopeNameAsync(new string[] { "test" }).ConfigureAwait(false);

            identityStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == "Id eq 'test'"), default));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out Mock<IAdminStore<ApiApiScope>> apiApiScopeStoreMock,
                out ResourceStore sut);

            apiApiScopeStoreMock.Setup(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == $"{nameof(ApiScope.Id)} eq 'test'"), default)).ReturnsAsync(new PageResponse<ApiApiScope>
                {
                    Count = 1,
                    Items = new[]
                    {
                        new ApiApiScope
                        {
                            ApiId = "test"
                        }
                    }
                });

            apiStoreMock.Setup(m => m.GetAsync("test", It.IsAny<GetRequest>(), default)).ReturnsAsync(new ProtectResource());

            await sut.FindApiResourcesByScopeNameAsync(new string[] { "test" }).ConfigureAwait(false);

            apiApiScopeStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Filter == $"{nameof(ApiScope.Id)} eq 'test'"), default));
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out Mock<IAdminStore<ApiApiScope>> apiApiScopeStoreMock,
                out ResourceStore sut);

            await sut.FindApiResourcesByNameAsync(new string[] { "test" }).ConfigureAwait(false);

            apiStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Expand == $"{nameof(ProtectResource.ApiClaims)},{nameof(ProtectResource.Secrets)},{nameof(ProtectResource.ApiScopes)},{nameof(ProtectResource.Properties)},{nameof(ProtectResource.Resources)}"), default));
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_should_call_store_GetAsync()
        {
            CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock,
                out Mock<IAdminStore<IdentityResource>> identityStoreMock,
                out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
                out Mock<IAdminStore<ApiApiScope>> apiApiScopeStoreMock,
                out ResourceStore sut);

            await sut.FindApiScopesByNameAsync(new string[] { "test" }).ConfigureAwait(false);

            apiScopeStoreMock.Verify(m => m.GetAsync(It.Is<PageRequest>(p =>
                p.Expand == $"{nameof(ApiScope.ApiScopeClaims)},{nameof(ApiScope.Properties)},{nameof(ApiScope.Resources)}"), default));
        }

        private static void CreateSut(out Mock<IAdminStore<ProtectResource>> apiStoreMock, 
            out Mock<IAdminStore<IdentityResource>> identityStoreMock, 
            out Mock<IAdminStore<ApiScope>> apiScopeStoreMock,
            out Mock<IAdminStore<ApiApiScope>> apiApiScopeStoreMock,
            out ResourceStore sut)
        {
            apiStoreMock = new Mock<IAdminStore<ProtectResource>>();
            identityStoreMock = new Mock<IAdminStore<IdentityResource>>();
            apiScopeStoreMock = new Mock<IAdminStore<ApiScope>>();
            apiApiScopeStoreMock = new Mock<IAdminStore<ApiApiScope>>();

            sut = new ResourceStore(apiStoreMock.Object, identityStoreMock.Object, apiScopeStoreMock.Object, apiApiScopeStoreMock.Object);

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

            apiScopeStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default))
                .ReturnsAsync(new PageResponse<ApiScope>
                {
                    Items = new List<ApiScope>(0)
                }).Verifiable();
        }
    }
}
