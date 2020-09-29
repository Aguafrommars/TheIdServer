using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore
{
    /// <summary>
    /// Interface used to store instances of <see cref="DataProtectionKey"/> in a <see cref="DbContext"/>
    /// </summary>
    public interface IKeyRotationContext
    {
        /// <summary>
        /// A collection of <see cref="DataProtectionKey"/>
        /// </summary>
        DbSet<KeyRotationKey> KeyRotationKeys { get; }
    }
}
