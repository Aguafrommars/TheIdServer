// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class UserConsentStore : GrantStore<UserConsent, Consent>, IUserConsentStore
    {
        public UserConsentStore(IAdminStore<UserConsent> store, IPersistentGrantSerializer serializer)
            : base(store, serializer)
        {
        }

        public Task<Consent> GetUserConsentAsync(string subjectId, string clientId, CancellationToken ct)
            => GetAsync(subjectId, clientId, ct);

        public Task RemoveUserConsentAsync(string subjectId, string clientId, CancellationToken ct)
            => RemoveAsync(subjectId, clientId, ct);

        public Task StoreUserConsentAsync(Consent consent, CancellationToken ct)
            => StoreAsync(consent, consent.Expiration, ct);

        protected override string GetClientId(Consent dto)
            => dto?.ClientId;

        protected override string GetSubjectId(Consent dto)
            => dto?.SubjectId;
    }
}
