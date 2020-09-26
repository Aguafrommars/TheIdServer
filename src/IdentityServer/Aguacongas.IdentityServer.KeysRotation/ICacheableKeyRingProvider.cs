using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    interface ICacheableKeyRingProvider
    {
        CacheableKeyRing GetCacheableKeyRing(DateTimeOffset now);
    }
}
