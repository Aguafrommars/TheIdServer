using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ICacheableKeyRingProvider : IKeyRingProvider
    {
        IKeyManager KeyManager { get; }
        IKeyRing RefreshCurrentKeyRing();
        CacheableKeyRing GetCacheableKeyRing(DateTimeOffset now);
    }
}
