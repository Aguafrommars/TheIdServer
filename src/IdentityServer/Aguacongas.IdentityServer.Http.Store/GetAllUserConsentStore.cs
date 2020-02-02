using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Models;
using IdentityServer4.Stores.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class GetAllUserConsentStore : IGetAllUserConsentStore
    {
        private readonly IAdminStore<UserConsent> _store;
        private readonly IPersistentGrantSerializer _serializer;

        public GetAllUserConsentStore(IAdminStore<UserConsent> store, IPersistentGrantSerializer serializer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<IEnumerable<Consent>> GetAllUserConsent(string subjectId)
        {
            return (await _store.GetAsync(new PageRequest
            {
                Filter = $"UserId eq '{subjectId}'",
                Select = "Data"
            }).ConfigureAwait(false)).Items
                .Select(c => _serializer.Deserialize<Consent>(c.Data));
        }
    }
}
