using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ICacheableKeyRingProvider : IKeyRingProvider
    {

        IKeyRing RefreshCurrentKeyRing();
        CacheableKeyRing GetCacheableKeyRing(DateTimeOffset now);
    }
}
