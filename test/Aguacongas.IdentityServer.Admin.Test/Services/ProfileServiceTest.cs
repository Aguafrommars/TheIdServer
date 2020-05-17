using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class ProfileServiceTest
    {
        [Fact]
        public async Task GetProfileDataAsync_should_resolve_provider_type()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();

            var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                UserName = "test"
            };
            await manager.CreateAsync(user);

            var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtClaimTypes.Subject, user.Id) })),
                new Client(), "test", 
                new string[] { "test" })
            {
                RequestedResources = new Resources
                {
                    IdentityResources = new List<IdentityResource>
                    {
                        new IdentityResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).AssemblyQualifiedName
                            }
                        }
                    },                    
                }
            };

            var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
                provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

            await sut.GetProfileDataAsync(context);

            Assert.Contains(context.IssuedClaims, c => c.Type == "test");
        }

        [Fact]
        public async Task GetProfileDataAsync_should_load_assemby_from_path()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();

            var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                UserName = "test"
            };
            await manager.CreateAsync(user);

            var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtClaimTypes.Subject, user.Id) })),
                new Client(), "test",
                new string[] { "test" })
            {
                RequestedResources = new Resources
                {
                    ApiResources = new List<ApiResource>
                    {
                        new ApiResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).AssemblyQualifiedName,
                                [ProfileServiceProperties.ClaimProviderAssemblyPathKey] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll"
                            }
                        }
                    },
                }
            };

            var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
                provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

            await sut.GetProfileDataAsync(context);

            Assert.Contains(context.IssuedClaims, c => c.Type == "test");
        }

        [Fact]
        public async Task GetProfileDataAsync_should_get_user_from_context()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();

            var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                UserName = "test"
            };
            await manager.CreateAsync(user);

            var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtClaimTypes.Subject, "not_found") })),
                new Client(), "test",
                new string[] { "test" })
            {
                RequestedResources = new Resources
                {
                    ApiResources = new List<ApiResource>
                    {
                        new ApiResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).AssemblyQualifiedName,
                                [ProfileServiceProperties.ClaimProviderAssemblyPathKey] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll"
                            }
                        }
                    },
                }
            };

            var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
                provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

            await sut.GetProfileDataAsync(context);

            Assert.Contains(context.IssuedClaims, c => c.Type == "test");
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
