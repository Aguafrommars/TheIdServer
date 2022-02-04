// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// public static class KeyManagement
    /// </summary>
    public class NullKeyManager : IKeyManager
    {
        /// <summary>
        /// public NullKeyManager()
        /// </summary>
        /// <param name="activationDate"></param>
        /// <param name="expirationDate"></param>
        /// <returns></returns>
        public IKey CreateNewKey(DateTimeOffset activationDate, DateTimeOffset expirationDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// public IKey CreateNewKey(
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IKey> GetAllKeys()
        {
            return Array.Empty<IKey>();
        }

        /// <summary>
        /// public
        /// </summary>
        /// <returns></returns>
        public CancellationToken GetCacheExpirationToken()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// public void SetCacheExpirationToken(
        /// </summary>
        /// <param name="revocationDate"></param>
        /// <param name="reason"></param>
        public void RevokeAllKeys(DateTimeOffset revocationDate, string reason = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// public void RevokeAllKeys(
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="reason"></param>
        public void RevokeKey(Guid keyId, string reason = null)
        {
            throw new NotImplementedException();
        }
    }
}
