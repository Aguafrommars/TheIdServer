// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class ProxyClaimsProviderTest
    {
        [Fact]
        public async Task GetAsync_should_resolve_provider_type_from_di()
        {
            var clientStoreMock = new Mock<IClientStore>();
            clientStoreMock.Setup(m => m.FindClientByIdAsync("test")).ReturnsAsync(new Client());
            var resourceStoreMock = new Mock<IResourceStore>();
            resourceStoreMock.Setup(m => m.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new[] { new ApiResource() });
            resourceStoreMock.Setup(m => m.FindIdentityResourcesByScopeNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(Array.Empty<IdentityResource>());

            var services = new ServiceCollection()
                .AddLogging()
                .AddTransient<IProvideClaims, ClaimsProvider>()
                .AddTransient(p => clientStoreMock.Object)
                .AddTransient(p => resourceStoreMock.Object)
                .AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();
            var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = "test",
                UserName = "test"
            };
            await manager.CreateAsync(user);

            var sut = new ProxyClaimsProvider<IdentityUser>(provider.GetService<IEnumerable<IProvideClaims>>(), 
                provider.GetRequiredService<IResourceStore>(),
                provider.GetRequiredService<IClientStore>(),
                provider.GetRequiredService<UserManager<IdentityUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>());

            var result = await sut.GetAsync("test", "test", "test", "test", typeof(ClaimsProvider).FullName).ConfigureAwait(false);

            Assert.Contains(result.Items, c => c.ClaimType == "test");
        }

        [Fact]
        public async Task GetAsync_should_load_assemby_from_path()
        {
            var clientStoreMock = new Mock<IClientStore>();
            clientStoreMock.Setup(m => m.FindClientByIdAsync("test")).ReturnsAsync(new Client());
            var resourceStoreMock = new Mock<IResourceStore>();
            resourceStoreMock.Setup(m => m.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new[] {new ApiResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderAssemblyPathKey] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll"
                            }
                        }});
            resourceStoreMock.Setup(m => m.FindIdentityResourcesByScopeNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(Array.Empty<IdentityResource>());

            var services = new ServiceCollection()
                .AddLogging()
                .AddTransient(p => clientStoreMock.Object)
                .AddTransient(p => resourceStoreMock.Object)
                .AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();

            var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = "test",
                UserName = "test"
            };
            await manager.CreateAsync(user);

            var sut = new ProxyClaimsProvider<IdentityUser>(provider.GetService<IEnumerable<IProvideClaims>>(),
                provider.GetRequiredService<IResourceStore>(),
                provider.GetRequiredService<IClientStore>(),
                provider.GetRequiredService<UserManager<IdentityUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>());

            var result = await sut.GetAsync("test", "test", "test", "test", typeof(ClaimsProvider).FullName).ConfigureAwait(false);

            Assert.Contains(result.Items, c => c.ClaimType == "test");
        }

        [Fact]
        public async Task GetAsync_should_get_claims_from_identity_resources()
        {
            var clientStoreMock = new Mock<IClientStore>();
            clientStoreMock.Setup(m => m.FindClientByIdAsync("test")).ReturnsAsync(new Client());
            var resourceStoreMock = new Mock<IResourceStore>();
            resourceStoreMock.Setup(m => m.FindApiResourcesByNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(Array.Empty<ApiResource>());
            resourceStoreMock.Setup(m => m.FindIdentityResourcesByScopeNameAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new IdentityResource[]
                    {
                        new IdentityResource()
                    });

            var services = new ServiceCollection()
                .AddLogging()
                .AddTransient<IProvideClaims, ClaimsProvider>()
                .AddTransient(p => clientStoreMock.Object)
                .AddTransient(p => resourceStoreMock.Object)
                .AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();

            var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = "test",
                UserName = "test"
            };
            await manager.CreateAsync(user);

            var sut = new ProxyClaimsProvider<IdentityUser>(provider.GetService<IEnumerable<IProvideClaims>>(),
                provider.GetRequiredService<IResourceStore>(),
                provider.GetRequiredService<IClientStore>(),
                provider.GetRequiredService<UserManager<IdentityUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>());

            var result = await sut.GetAsync("test", "test", "test", "test", typeof(ClaimsProvider).FullName).ConfigureAwait(false);

            Assert.Contains(result.Items, c => c.ClaimType == "test");
        }

        class ClaimsProvider : IProvideClaims
        {
            public Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, Client client, string caller, Resource resource)
            {
                return Task.FromResult(new Claim[] { new Claim("test", "test") } as IEnumerable<Claim>);
            }
        }
    }
}
