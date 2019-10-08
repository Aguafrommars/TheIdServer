using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class AuthorizationCodeStore : IAuthorizationCodeStore
    {
        public Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAuthorizationCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
        {
            throw new NotImplementedException();
        }
    }
}
