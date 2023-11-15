using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store;
public class PushedAuthorizationRequestStore : IPushedAuthorizationRequestStore
{
    public Task ConsumeByHashAsync(string referenceValueHash)
    {
        throw new NotImplementedException();
    }

    public Task<PushedAuthorizationRequest> GetByHashAsync(string referenceValueHash)
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync(PushedAuthorizationRequest pushedAuthorizationRequest)
    {
        throw new NotImplementedException();
    }
}
