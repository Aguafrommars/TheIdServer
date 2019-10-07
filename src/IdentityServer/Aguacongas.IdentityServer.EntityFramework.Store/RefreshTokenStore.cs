using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class RefreshTokenStore : IRefreshTokenStore
    {
        public Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
