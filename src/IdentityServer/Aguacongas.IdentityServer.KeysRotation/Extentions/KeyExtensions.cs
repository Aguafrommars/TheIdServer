using System;

namespace Microsoft.AspNetCore.DataProtection.KeyManagement
{
    internal static class KeyExtensions
    {
        public static bool IsExpired(this IKey key, DateTimeOffset now)
        {
            return (key.ExpirationDate <= now);
        }
    }
}
