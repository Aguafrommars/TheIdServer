// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class EntityFrameworkCoreXmlRepositoryTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new EntityFrameworkCoreXmlRepository<OperationalDbContext>(null, null));
            Assert.Throws<ArgumentNullException>(() => new EntityFrameworkCoreXmlRepository<OperationalDbContext>(null, NullLoggerFactory.Instance));
        }

        [Fact]
        public void GetAllElements_should_return_deserialization_error_as_null()
        {
            var dbName = Guid.NewGuid().ToString();
            var provider = new ServiceCollection()
                .AddDbContext<OperationalDbContext>(o => o.UseInMemoryDatabase(dbName))
                .BuildServiceProvider(validateScopes: true);

            using (var scope = provider.CreateScope())
            {
                using var context = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
                context.KeyRotationKeys.Add(new KeyRotationKey
                {
                    FriendlyName = "test",
                    Xml = "non desialiazable"
                });
                context.SaveChanges();
            }

            var sut = new EntityFrameworkCoreXmlRepository<OperationalDbContext>(provider, NullLoggerFactory.Instance);

            var result = sut.GetAllElements();

            Assert.Null(result.First());
        }
    }
}
