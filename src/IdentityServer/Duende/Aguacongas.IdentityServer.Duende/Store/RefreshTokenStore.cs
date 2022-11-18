// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using IsModels = Duende.IdentityServer.Models;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class RefreshTokenStore : GrantStore<RefreshToken, IsModels.RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(IAdminStore<RefreshToken> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<IsModels.RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
            => GetAsync(refreshTokenHandle);

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
            => RemoveAsync(refreshTokenHandle);

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task<string> StoreRefreshTokenAsync(IsModels.RefreshToken refreshToken)
            => StoreAsync(refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime));

        public Task UpdateRefreshTokenAsync(string handle, IsModels.RefreshToken refreshToken)
            => UpdateAsync(handle, refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime));

        protected override string GetClientId(IsModels.RefreshToken dto)
            => dto?.ClientId;

        protected override string GetSubjectId(IsModels.RefreshToken dto)
            => dto?.SubjectId;

        protected override RefreshToken CreateEntity(IsModels.RefreshToken dto, string clientId, string subjectId, DateTime? expiration)
        {
            var entity = base.CreateEntity(dto, clientId, subjectId, expiration);
            entity.SessionId = dto.SessionId;
            return entity;
        }
    }
}
