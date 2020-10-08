using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Aguacongas.IdentityServer.Admin.Services
{
    class NullKeyManager : IKeyManager
    {
        public IKey CreateNewKey(DateTimeOffset activationDate, DateTimeOffset expirationDate)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IKey> GetAllKeys()
        {
            return Array.Empty<IKey>();
        }

        public CancellationToken GetCacheExpirationToken()
        {
            throw new NotImplementedException();
        }

        public void RevokeAllKeys(DateTimeOffset revocationDate, string reason = null)
        {
            throw new NotImplementedException();
        }

        public void RevokeKey(Guid keyId, string reason = null)
        {
            throw new NotImplementedException();
        }
    }
}
