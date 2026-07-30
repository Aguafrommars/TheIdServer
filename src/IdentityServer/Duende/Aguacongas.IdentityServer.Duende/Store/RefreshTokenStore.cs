// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;
using IsModels = Duende.IdentityServer.Models;

namespace Aguacongas.IdentityServer.Store
{
    public class RefreshTokenStore : GrantStore<RefreshToken, IsModels.RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(IAdminStore<RefreshToken> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<IsModels.RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle, CancellationToken ct)
            => GetAsync(refreshTokenHandle, ct);

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle, CancellationToken ct)
            => RemoveAsync(refreshTokenHandle, ct);

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId, CancellationToken ct)
            => RemoveAsync(subjectId, clientId, ct);

        public Task<string> StoreRefreshTokenAsync(IsModels.RefreshToken refreshToken, CancellationToken ct)
            => StoreAsync(refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime), ct);

        public Task UpdateRefreshTokenAsync(string handle, IsModels.RefreshToken refreshToken, CancellationToken ct)
            => UpdateAsync(handle, refreshToken, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime), ct);

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
