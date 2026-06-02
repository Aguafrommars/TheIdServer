using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store;

public class PushedAuthorizationRequestStore : IPushedAuthorizationRequestStore
{
    private readonly IAdminStore<Entity.PushedAuthorizationRequest> _store;

    public PushedAuthorizationRequestStore(IAdminStore<Entity.PushedAuthorizationRequest> store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
    }

    public Task ConsumeByHashAsync(string referenceValueHash, CancellationToken ct)
    => _store.DeleteAsync(referenceValueHash, ct);

    public async Task<PushedAuthorizationRequest> GetByHashAsync(string referenceValueHash, CancellationToken ct)
    {
        var entity = await _store.GetAsync(referenceValueHash, null, ct).ConfigureAwait(false);
        return entity is null ? null : new PushedAuthorizationRequest
        {
            ExpiresAtUtc = entity.ExpiresAtUtc,
            Parameters = entity.Parameters,
            ReferenceValueHash = entity.Id
        };
    }

    public Task StoreAsync(PushedAuthorizationRequest pushedAuthorizationRequest, CancellationToken ct)
    => _store.CreateAsync(new Entity.PushedAuthorizationRequest
    {
        ExpiresAtUtc = pushedAuthorizationRequest.ExpiresAtUtc,
        Id = pushedAuthorizationRequest.ReferenceValueHash,
        Parameters = pushedAuthorizationRequest.Parameters
    }, ct);
}
