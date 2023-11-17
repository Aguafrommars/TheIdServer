using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
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

    public Task ConsumeByHashAsync(string referenceValueHash)
    => _store.DeleteAsync(referenceValueHash);
    

    public async Task<PushedAuthorizationRequest> GetByHashAsync(string referenceValueHash)
    {
        var entity = await _store.GetAsync(referenceValueHash, null).ConfigureAwait(false);
        return entity is null ? null : new PushedAuthorizationRequest
        {
            ExpiresAtUtc = entity.ExpiresAtUtc,
            Parameters = entity.Parameters,
            ReferenceValueHash = entity.Id
        };
    }

    public Task StoreAsync(PushedAuthorizationRequest pushedAuthorizationRequest)
    => _store.CreateAsync(new Entity.PushedAuthorizationRequest
    {
        ExpiresAtUtc = pushedAuthorizationRequest.ExpiresAtUtc,
        Id = pushedAuthorizationRequest.ReferenceValueHash,
        Parameters = pushedAuthorizationRequest.Parameters
    });
}
