// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System;
#if DUENDE
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using models = Duende.IdentityServer.Models;
#else
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using models = IdentityServer4.Models;
#endif
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class RefreshTokenStore : GrantStore<RefreshToken, models.RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(IAdminStore<RefreshToken> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<models.RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
            => GetAsync(refreshTokenHandle);

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
            => RemoveAsync(refreshTokenHandle);

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task<string> StoreRefreshTokenAsync(models.RefreshToken refreshToken)
            => StoreAsync(refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime));

        public Task UpdateRefreshTokenAsync(string handle, models.RefreshToken refreshToken)
            => UpdateAsync(handle, refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime));

        protected override string GetClientId(models.RefreshToken dto)
            => dto?.ClientId;

        protected override string GetSubjectId(models.RefreshToken dto)
            => dto?.SubjectId;

        protected override RefreshToken CreateEntity(models.RefreshToken dto, string clientId, string subjectId, DateTime? expiration)
        {
            var entity = base.CreateEntity(dto, clientId, subjectId, expiration);
            entity.SessionId = dto.SessionId;
            return entity;
        }
    }
}
