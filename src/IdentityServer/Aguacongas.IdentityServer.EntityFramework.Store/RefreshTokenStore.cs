using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System.Threading.Tasks;
using Models = IdentityServer4.Models;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class RefreshTokenStore : GrantStore<RefreshToken, Models.RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(IdentityServerDbContext context, IPersistentGrantSerializer serializer) : base(context, serializer)
        {
        }

        public Task<Models.RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
            => GetAsync(refreshTokenHandle);

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
            => RemoveAsync(refreshTokenHandle);

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task<string> StoreRefreshTokenAsync(Models.RefreshToken refreshToken)
            => StoreAsync(refreshToken);

        public Task UpdateRefreshTokenAsync(string handle, Models.RefreshToken refreshToken)
            => UpdateAsync(handle, refreshToken);

        protected override string GetClientId(Models.RefreshToken dto)
            => dto?.ClientId;

        protected override string GetSubjectId(Models.RefreshToken dto)
            => dto?.SubjectId;
    }
}
