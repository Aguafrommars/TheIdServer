using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Interface used to store instances of <see cref="DataProtectionKey"/> in a <see cref="DbContext"/>
    /// </summary>
    public interface IKeyRotationContext
    {
        /// <summary>
        /// A collection of <see cref="DataProtectionKey"/>
        /// </summary>
        DbSet<DataProtectionKey> KeyRotationKeys { get; }
    }
}
