using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// <see cref="IPersistedGrantService"/> implementation
    /// </summary>
    /// <seealso cref="IPersistedGrantService" />
    public class PersistedGrantService : IPersistedGrantService
    {
        private readonly IGetAllUserConsentStore _getAllUserConsentStore;
        private readonly IUserConsentStore _userConsentStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantService"/> class.
        /// </summary>
        /// <param name="getAllUserConsentStore">The get all user consent store.</param>
        /// <param name="userConsentStore">The user consent store.</param>
        /// <exception cref="ArgumentNullException">
        /// getAllUserConsentStore
        /// or
        /// userConsentStore
        /// </exception>
        public PersistedGrantService(IGetAllUserConsentStore getAllUserConsentStore, IUserConsentStore userConsentStore)
        {
            _getAllUserConsentStore = getAllUserConsentStore ?? throw new ArgumentNullException(nameof(getAllUserConsentStore));
            _userConsentStore = userConsentStore ?? throw new ArgumentNullException(nameof(userConsentStore));
        }
        /// <summary>
        /// Gets all grants for a given subject ID.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public Task<IEnumerable<Consent>> GetAllGrantsAsync(string subjectId)
            => _getAllUserConsentStore.GetAllUserConsent(subjectId);

        /// <summary>
        /// Removes all grants for a given subject id and client id combination.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveAllGrantsAsync(string subjectId, string clientId)
            => _userConsentStore.RemoveUserConsentAsync(subjectId, clientId);
    }
}
