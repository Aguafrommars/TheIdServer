using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class UserConsentStore : IUserConsentStore
    {
        public Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task StoreUserConsentAsync(Consent consent)
        {
            throw new NotImplementedException();
        }
    }
}
