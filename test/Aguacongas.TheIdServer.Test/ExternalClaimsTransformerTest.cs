// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Test
{
    public class ExternalClaimsTransformerTest
    {
        [Fact]
        public async Task TransformPrincipal_should_transform_claims()
        {
            var builder = CreateServices().BuildServiceProvider();

            using var scope = builder.CreateScope();
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            configurationDbContext.ExternalClaimTransformations.Add(new ExternalClaimTransformation
            {
                Id = Guid.NewGuid().ToString(),
                FromClaimType = "test",
                ToClaimType = "transformed",
                Scheme = "test"
            });
            var serializer = builder.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            configurationDbContext.Providers.Add(new SchemeDefinition
            {
                Id = "test",
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            });
            configurationDbContext.SaveChanges();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("test", "test"),
                new Claim("not transformed", "test"),
            }));

            var sut = builder.GetRequiredService<ExternalClaimsTransformer<ApplicationUser>>();

            var result = await sut.TransformPrincipalAsync(principal, "test").ConfigureAwait(false);

            Assert.Contains(result.Claims, c => c.Type == "transformed");
        }

        [Theory]
        [InlineData(JwtClaimTypes.Subject)]
        [InlineData(ClaimTypes.NameIdentifier)]
        public async Task TransformPrincipal_should_provision_user(string claimType)
        {
            var builder = CreateServices().BuildServiceProvider();

            using var scope = builder.CreateScope();
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var serializer = builder.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            configurationDbContext.Providers.Add(new SchemeDefinition
            {
                Id = "test",
                StoreClaims = true,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            });
            configurationDbContext.SaveChanges();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(claimType, "test")
            }));

            var sut = builder.GetRequiredService<ExternalClaimsTransformer<ApplicationUser>>();

            var result = await sut.TransformPrincipalAsync(principal, "test").ConfigureAwait(false);

            Assert.Contains(result.Claims, c => c.Type == claimType);
            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Assert.NotNull(applicationDbContext.UserClaims.FirstOrDefaultAsync(c => c.ClaimType == claimType));
        }

        [Fact]
        public async Task TransformPrincipal_should_throw_when_id_not_found()
        {
            var builder = CreateServices().BuildServiceProvider();

            using var scope = builder.CreateScope();
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var serializer = builder.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            configurationDbContext.Providers.Add(new SchemeDefinition
            {
                Id = "test",
                StoreClaims = true,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            });
            configurationDbContext.SaveChanges();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>()));

            var sut = builder.GetRequiredService<ExternalClaimsTransformer<ApplicationUser>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TransformPrincipalAsync(principal, "test"))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task TransformPrincipal_should_throw_when_cannot_create_user()
        {
            var services = CreateServices();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userStoreMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError{ Code = "test", Description = "test" }
                }));
            
            userStoreMock.As<IUserLoginStore<ApplicationUser>>()
                .Setup(m => m.FindByLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ApplicationUser);

            services.AddTransient(p => userStoreMock.Object);
            var builder = services.BuildServiceProvider();

            using var scope = builder.CreateScope();
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var serializer = builder.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            configurationDbContext.Providers.Add(new SchemeDefinition
            {
                Id = "test",
                StoreClaims = true,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            });
            configurationDbContext.SaveChanges();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtClaimTypes.Subject, "test")
            }));

            var sut = builder.GetRequiredService<ExternalClaimsTransformer<ApplicationUser>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TransformPrincipalAsync(principal, "test"))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task TransformPrincipal_should_throw_when_cannot_create_login()
        {
            var services = CreateServices();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userStoreMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>())).ReturnsAsync(IdentityResult.Success);
            var userLoginStoreMock = userStoreMock.As<IUserLoginStore<ApplicationUser>>();

            userLoginStoreMock.Setup(m => m.FindByLoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ApplicationUser);

            userLoginStoreMock.Setup(m => m.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserLoginInfo>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            userStoreMock.Setup(m => m.GetNormalizedUserNameAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("test");

            userStoreMock.Setup(m => m.GetUserNameAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("test");

            userStoreMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError { Code = "test", Description = "test" }
                }));

            services.AddTransient(p => userStoreMock.Object);
            var builder = services.BuildServiceProvider();

            using var scope = builder.CreateScope();
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var serializer = builder.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            configurationDbContext.Providers.Add(new SchemeDefinition
            {
                Id = "test",
                StoreClaims = true,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            });
            configurationDbContext.SaveChanges();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtClaimTypes.Subject, "test")
            }));

            var sut = builder.GetRequiredService<ExternalClaimsTransformer<ApplicationUser>>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.TransformPrincipalAsync(principal, "test"))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task TransformPrincipal_should_add_remove_claims()
        {
            var builder = CreateServices().BuildServiceProvider();

            using var scope = builder.CreateScope();
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var serializer = builder.GetRequiredService<IAuthenticationSchemeOptionsSerializer>();
            configurationDbContext.Providers.Add(new SchemeDefinition
            {
                Id = "test",
                StoreClaims = true,
                SerializedHandlerType = serializer.SerializeType(typeof(GoogleHandler)),
                SerializedOptions = serializer.SerializeOptions(new GoogleOptions(), typeof(GoogleOptions))
            });
            configurationDbContext.SaveChanges();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test"),
                new Claim("test", "test"),
            }));

            var sut = builder.GetRequiredService<ExternalClaimsTransformer<ApplicationUser>>();

            await sut.TransformPrincipalAsync(principal, "test").ConfigureAwait(false);

            principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test"),
                new Claim("new", "test"),
            }));

            await sut.TransformPrincipalAsync(principal, "test").ConfigureAwait(false);

            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Assert.NotNull(applicationDbContext.UserClaims.FirstOrDefault(c => c.ClaimType == "new"));
            Assert.Null(applicationDbContext.UserClaims.FirstOrDefault(c => c.ClaimType == "test"));
        }

        private static IServiceCollection CreateServices()
        {
            var configuration = new ConfigurationBuilder().Build();
            var dbId = Guid.NewGuid().ToString();
            var services = new ServiceCollection()
                .AddTransient<IConfiguration>(p => configuration)
                .AddLogging()
                .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(dbId))
                .AddIdentityServer4AdminEntityFrameworkStores<ApplicationUser, ApplicationDbContext>()
                .AddConfigurationEntityFrameworkStores(options => options.UseInMemoryDatabase(dbId))
                .AddIdentityProviderStore();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR();

            services.AddControllersWithViews()
                .AddIdentityServerAdmin<ApplicationUser, SchemeDefinition>()
                .AddEntityFrameworkStore<ConfigurationDbContext>();

            return services;
        }
    }
}
