// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Services;

public class PreRenderCultureStore(IServiceProvider provider) : IReadOnlyCultureStore
{
    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public async Task<PageResponse<Culture>> GetAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IAdminStore<Culture>>();
        return await store.GetAsync(pageRequest, cancellationToken).ConfigureAwait(false); // await is needed here else connection is diposed
    }
}
