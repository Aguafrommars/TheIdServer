using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class UserConsentStore : GrantStore<UserConsent, Consent>, IUserConsentStore
    {
        public UserConsentStore(IAdminStore<UserConsent> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
            => GetAsync(subjectId, clientId);

        public Task RemoveUserConsentAsync(string subjectId, string clientId)
            => RemoveAsync(subjectId, clientId);

        public Task StoreUserConsentAsync(Consent consent)
            => StoreAsync(consent, consent.Expiration);

        protected override string GetClientId(Consent dto) 
            => dto?.ClientId;

        protected override string GetSubjectId(Consent dto) 
            => dto?.SubjectId;
    }
}
