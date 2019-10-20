using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    public class PersistedGrantService : IPersistedGrantService
    {
        private readonly IGetAllUserConsentStore _getAllUserConsentStore;
        private readonly IUserConsentStore _userConsentStore;

        public PersistedGrantService(IGetAllUserConsentStore getAllUserConsentStore, IUserConsentStore userConsentStore)
        {
            _getAllUserConsentStore = getAllUserConsentStore ?? throw new ArgumentNullException(nameof(getAllUserConsentStore));
            _userConsentStore = userConsentStore ?? throw new ArgumentNullException(nameof(userConsentStore));
        }
        public Task<IEnumerable<Consent>> GetAllGrantsAsync(string subjectId)
            => _getAllUserConsentStore.GetAllUserConsent(subjectId);

        public Task RemoveAllGrantsAsync(string subjectId, string clientId)
            => _userConsentStore.RemoveUserConsentAsync(subjectId, clientId);
    }
}
