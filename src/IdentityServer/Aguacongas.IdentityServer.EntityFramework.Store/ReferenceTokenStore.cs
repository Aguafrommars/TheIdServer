using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ReferenceTokenStore : GrantStore<ReferenceToken, Token>, IReferenceTokenStore
    {
        public ReferenceTokenStore(OperationalDbContext context, IPersistentGrantSerializer serializer, ILogger<ReferenceTokenStore> logger)
            : base(context, serializer, logger)
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
