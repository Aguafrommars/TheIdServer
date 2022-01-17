using Aguacongas.IdentityServer.Validators;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Duende.Test.Validators
{
    public class BackchannelAuthenticationUserValidatorTest
    {
        [Fact]
        public void Constructor_should_chack_arguments()
        {
            Assert.Throws<ArgumentNullException>(() => new BackchannelAuthenticationUserValidator(null));
        }

        [Fact]
        public async Task ValidateRequestAsync_should_validate_user_from_loginHint()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userStoreMock.Setup(m => m.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                    Email = $"{Guid.NewGuid()}@theidserver.com",
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddTransient(p => userStoreMock.Object)
                .AddIdentityCore<ApplicationUser>();
            var provider = serviceCollection.BuildServiceProvider();

            var sut = new BackchannelAuthenticationUserValidator(provider.GetRequiredService<UserManager<ApplicationUser>>());

            var result = await sut.ValidateRequestAsync(new BackchannelAuthenticationUserValidatorContext
            {
                LoginHint = Guid.NewGuid().ToString()
            });

            Assert.Null(result.Error);

            result = await sut.ValidateRequestAsync(new BackchannelAuthenticationUserValidatorContext
            {
                LoginHintToken = Guid.NewGuid().ToString()
            });

            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task ValidateRequestAsync_should_validate_user_from_loginTokenHint()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userStoreMock.Setup(m => m.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                    Email = $"{Guid.NewGuid()}@theidserver.com",
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddTransient(p => userStoreMock.Object)
                .AddIdentityCore<ApplicationUser>();
            var provider = serviceCollection.BuildServiceProvider();

            var sut = new BackchannelAuthenticationUserValidator(provider.GetRequiredService<UserManager<ApplicationUser>>());

            var result = await sut.ValidateRequestAsync(new BackchannelAuthenticationUserValidatorContext
            {
                LoginHintToken = Guid.NewGuid().ToString()
            });

            Assert.Null(result.Error);

            result = await sut.ValidateRequestAsync(new BackchannelAuthenticationUserValidatorContext
            {
                LoginHint = Guid.NewGuid().ToString()
            });

            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task ValidateRequestAsync_should_validate_user_from_idTokenHint()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userStoreMock.Setup(m => m.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                    Email = $"{Guid.NewGuid()}@theidserver.com",
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddTransient(p => userStoreMock.Object)
                .AddIdentityCore<ApplicationUser>();
            var provider = serviceCollection.BuildServiceProvider();

            var sut = new BackchannelAuthenticationUserValidator(provider.GetRequiredService<UserManager<ApplicationUser>>());

            var result = await sut.ValidateRequestAsync(new BackchannelAuthenticationUserValidatorContext
            {
                IdTokenHintClaims = new [] { new Claim(JwtClaimTypes.Subject, Guid.NewGuid().ToString()) }
            });

            Assert.Null(result.Error);
        }
    }   
}
