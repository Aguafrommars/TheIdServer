using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;
using ISConfiguration = Duende.IdentityServer.Configuration;
using ISModels = Duende.IdentityServer.Models;

namespace Aguacongas.IdentityServer.EntityFramework.Store.Duende.Test;
public class PushedAuthorizationRequestStoreTest
{
    [Fact]
    public async Task ConsumeByHashAsync_should_delete_row()
    {
        CreateSut(out var _, out var context, out var sut);

        var id = GenerateId();
        await context.AddAsync(new PushedAuthorizationRequest
        {
            Id = id,
            ExpiresAtUtc = DateTime.Now,
            Parameters = GenerateId()
        });
        await context.SaveChangesAsync();

        await sut.ConsumeByHashAsync(id);

        var deleted = await context.PushedAuthorizationRequests.FirstOrDefaultAsync(par => par.Id == id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetByHashAsync_should_return_the_par()
    {
        CreateSut(out var _, out var context, out var sut);

        var id = GenerateId();
        await context.AddAsync(new PushedAuthorizationRequest
        {
            Id = id,
            ExpiresAtUtc = DateTime.Now,
            Parameters = GenerateId()
        });
        await context.SaveChangesAsync();

        var result = await sut.GetByHashAsync(id);

        Assert.NotNull(result);

        Assert.Null(await sut.GetByHashAsync(GenerateId()));
    }

    [Fact]
    public async Task StoreAsync_should_store_the_par()
    {
        CreateSut(out var _, out var context, out var sut);

        var id = GenerateId();

        await sut.StoreAsync(new ISModels.PushedAuthorizationRequest
        {
            ExpiresAtUtc = DateTime.UtcNow,
            Parameters = GenerateId(),
            ReferenceValueHash = id
        });

        var stored = await context.PushedAuthorizationRequests.FirstOrDefaultAsync(par => par.Id == id);

        Assert.NotNull(stored);
    }

    private static void CreateSut(out IServiceScope scope, out OperationalDbContext context, out PushedAuthorizationRequestStore sut)
    {
        var provider = new ServiceCollection()
            .AddLogging()
            .Configure<MemoryCacheOptions>(options => { })
            .Configure<ISConfiguration.IdentityServerOptions>(options => { })
            .AddTransient(p => p.GetRequiredService<IOptions<ISConfiguration.IdentityServerOptions>>().Value)
            .AddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>))
            .AddOperationalStores()
            .AddOperationalEntityFrameworkStores(options =>
                options.UseInMemoryDatabase(GenerateId()))
            .BuildServiceProvider();
        scope = provider.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        context = serviceProvider.GetRequiredService<OperationalDbContext>();
        sut = new PushedAuthorizationRequestStore(serviceProvider.GetRequiredService<IAdminStore<PushedAuthorizationRequest>>());
    }
    private static string GenerateId()
        => Guid.NewGuid().ToString();

}
