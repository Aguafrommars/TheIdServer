// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Test
{
    public class DeviceFlowStoreTest
    {
        [Fact]
        public void Constructor_should_validate_parameters()
        {
            var builder = new ServiceCollection()
                .AddOperationalEntityFrameworkStores(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();
            Assert.Throws<ArgumentNullException>(() => new DeviceFlowStore(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new DeviceFlowStore(builder.GetRequiredService<OperationalDbContext>(), null, null));
            Assert.Throws<ArgumentNullException>(() => 
                new DeviceFlowStore(builder.GetRequiredService<OperationalDbContext>(),
                builder.GetRequiredService<IPersistentGrantSerializer>(), null));
        }

        [Fact]
        public async Task FindByDeviceCodeAsync_should_return_DeviceCode()
        {
            CreateSut(out IServiceScope scope, out OperationalDbContext context, out DeviceFlowStore sut);

            using (scope)
            {
                var code = GenerateId();
                await context.DeviceCodes.AddAsync(new DeviceCode
                {
                    Id = GenerateId(),
                    ClientId = GenerateId(),
                    Data = "{}",
                    Code = code
                });
                await context.SaveChangesAsync();

                Assert.NotNull(await sut.FindByDeviceCodeAsync(code).ConfigureAwait(false));
                Assert.Null(await sut.FindByDeviceCodeAsync(GenerateId()).ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task FindByUserCodeAsync_should_return_DeviceCode()
        {
            CreateSut(out IServiceScope scope, out OperationalDbContext context, out DeviceFlowStore sut);

            using (scope)
            {
                var code = GenerateId();
                await context.DeviceCodes.AddAsync(new DeviceCode
                {
                    Id = GenerateId(),
                    ClientId = GenerateId(),
                    Data = "{}",
                    UserCode = code
                });
                await context.SaveChangesAsync();

                Assert.NotNull(await sut.FindByUserCodeAsync(code).ConfigureAwait(false));
                Assert.Null(await sut.FindByUserCodeAsync(GenerateId()).ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task RemoveByDeviceCodeAsync_should_delete_data_in_db()
        {
            CreateSut(out IServiceScope scope, out OperationalDbContext context, out DeviceFlowStore sut);

            using (scope)
            {
                var code = GenerateId();
                await context.DeviceCodes.AddAsync(new DeviceCode
                {
                    Id = GenerateId(),
                    ClientId = GenerateId(),
                    Data = "{}",
                    Code = code
                });
                await context.SaveChangesAsync();

                await sut.RemoveByDeviceCodeAsync(code).ConfigureAwait(false);
                await sut.RemoveByDeviceCodeAsync(code).ConfigureAwait(false);

                Assert.Null(await context.DeviceCodes.FirstOrDefaultAsync(d => d.Code == code));
            }
        }

        [Fact]
        public async Task StoreDeviceAuthorizationAsync_should_add_data_in_db()
        {
            CreateSut(out IServiceScope scope, out OperationalDbContext context, out DeviceFlowStore sut);

            var code = GenerateId();
            var userCode = GenerateId();
            var deviceCode = new IdentityServer4.Models.DeviceCode
            {
                ClientId = GenerateId(),
            };
            await sut.StoreDeviceAuthorizationAsync(code, userCode, deviceCode);

            using (scope)
            {
                Assert.NotNull(await context.DeviceCodes.FirstOrDefaultAsync(d => d.Code == code && d.UserCode == userCode));
            }
        }

        [Fact]
        public async Task UpdateByUserCodeAsync_should_uodate_data_in_db()
        {
            CreateSut(out IServiceScope scope, out OperationalDbContext context, out DeviceFlowStore sut);

            using (scope)
            {
                var code = GenerateId();
                var id = GenerateId();
                await context.DeviceCodes.AddAsync(new DeviceCode
                {
                    Id = id,
                    ClientId = GenerateId(),
                    UserCode = code,
                    Data = "{}",
                });
                await context.SaveChangesAsync();

                await sut.UpdateByUserCodeAsync(code, new IdentityServer4.Models.DeviceCode()).ConfigureAwait(false);

                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    sut.UpdateByUserCodeAsync(GenerateId(), new IdentityServer4.Models.DeviceCode()));
            }
        }

        private static void CreateSut(out IServiceScope scope, out OperationalDbContext context, out DeviceFlowStore sut)
        {
            var provider = new ServiceCollection()
                .AddLogging()
                .AddOperationalEntityFrameworkStores(options =>
                    options.UseInMemoryDatabase(GenerateId()))
                .BuildServiceProvider();
            scope = provider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            context = serviceProvider.GetRequiredService<OperationalDbContext>();
            sut = serviceProvider.GetRequiredService<DeviceFlowStore>();
        }

        private static string GenerateId()
            => Guid.NewGuid().ToString();
    }
}
