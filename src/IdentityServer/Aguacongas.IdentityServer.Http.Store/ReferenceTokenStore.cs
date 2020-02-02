using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ReferenceTokenStore : GrantStore<ReferenceToken, Token>, IReferenceTokenStore
    {
        public ReferenceTokenStore(IAdminStore<ReferenceToken> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<Token> GetReferenceTokenAsync(string handle)
            => GetAsync(handle);

        public Task RemoveReferenceTokenAsync(string handle)
            => RemoveAsync(handle);

        public Task RemoveReferenceTokensAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task<string> StoreReferenceTokenAsync(Token token)
            => StoreAsync(token, token.CreationTime.AddSeconds(token.Lifetime));

        protected override string GetClientId(Token dto) 
            => dto?.ClientId;

        protected override string GetSubjectId(Token dto)
            => dto?.SubjectId;
    }
}
