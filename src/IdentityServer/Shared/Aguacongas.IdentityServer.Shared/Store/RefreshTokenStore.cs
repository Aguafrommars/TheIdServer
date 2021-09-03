// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
#if DUENDE
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using Models = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Models = IdentityServer4.Models;
#endif
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class RefreshTokenStore : GrantStore<RefreshToken, Models.RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(IAdminStore<RefreshToken> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<Models.RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
            => GetAsync(refreshTokenHandle);

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
            => RemoveAsync(refreshTokenHandle);

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task<string> StoreRefreshTokenAsync(Models.RefreshToken refreshToken)
            => StoreAsync(refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime));

        public Task UpdateRefreshTokenAsync(string handle, Models.RefreshToken refreshToken)
            => UpdateAsync(handle, refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime));

        protected override string GetClientId(Models.RefreshToken dto)
            => dto?.ClientId;

        protected override string GetSubjectId(Models.RefreshToken dto)
            => dto?.SubjectId;
    }
}
