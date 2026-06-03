// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
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

namespace Aguacongas.IdentityServer.Admin.Test.Services;

public class ProfileServiceTest
{
    [Fact]
    public async Task GetProfileDataAsync_should_resolve_provider_type_from_di()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddTransient<IProvideClaims, ClaimsProvider>()
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

        var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity([new Claim(JwtClaimTypes.Subject, user.Id)])),
            new Client(), "test",
            ["test"])
        {
            RequestedResources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    IdentityResources =
                    [
                        new IdentityResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).FullName
                            }
                        }
                    ]
                }
            }
        };

        var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
            provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
            provider.GetService<IEnumerable<IProvideClaims>>(),
            provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

        await sut.GetProfileDataAsync(context, default);

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

        var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity([new Claim(JwtClaimTypes.Subject, user.Id)])),
            new Client(), "test",
            ["test"])
        {
            RequestedResources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    ApiResources =
                    [
                        new ApiResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).FullName,
                                [ProfileServiceProperties.ClaimProviderAssemblyPathKey] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll"
                            }
                        }
                    ]
                }
            }
        };

        var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
            provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
            provider.GetService<IEnumerable<IProvideClaims>>(),
            provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

        await sut.GetProfileDataAsync(context, default);

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

        var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity([new Claim(JwtClaimTypes.Subject, "not_found")])),
            new Client(), "test",
            ["test"])
        {
            RequestedResources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    ApiResources =
                    [
                        new ApiResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).FullName,
                                [ProfileServiceProperties.ClaimProviderAssemblyPathKey] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll"
                            }
                        }
                    ]
                }
            }
        };

        var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
            provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
            provider.GetService<IEnumerable<IProvideClaims>>(),
            provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

        await sut.GetProfileDataAsync(context, default);

        Assert.Contains(context.IssuedClaims, c => c.Type == "test");
    }

    [Fact]
    public async Task GetProfileDataAsync_should_get_add_act_claim()
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

        var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(JwtClaimTypes.Subject, "test"),
                new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.GrantTypes.TokenExchange),
                new Claim(JwtClaimTypes.Actor, "test"),
            ])),
            new Client(), "test",
            ["test"])
        {
            RequestedResources = new ResourceValidationResult
            {
                Resources = new Resources
                {
                    ApiResources =
                    [
                        new ApiResource
                        {
                            Properties = new Dictionary<string, string>
                            {
                                [ProfileServiceProperties.ClaimProviderTypeKey] = typeof(ClaimsProvider).FullName,
                                [ProfileServiceProperties.ClaimProviderAssemblyPathKey] = $"{typeof(ClaimsProvider).Assembly.GetName().Name}.dll"
                            }
                        }
                    ]
                }
            }
        };

        var sut = new ProfileService<IdentityUser>(provider.GetRequiredService<UserManager<IdentityUser>>(),
            provider.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>(),
            provider.GetService<IEnumerable<IProvideClaims>>(),
            provider.GetRequiredService<ILogger<ProfileService<IdentityUser>>>());

        await sut.GetProfileDataAsync(context, default);

        Assert.Contains(context.IssuedClaims, c => c.Type == JwtClaimTypes.Actor);
    }

    class ClaimsProvider : IProvideClaims
    {
        public Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, IConnectedApplication application, string caller, Resource resource)
        {
            return Task.FromResult(new Claim[] { new("test", "test") } as IEnumerable<Claim>);
        }
    }
}
