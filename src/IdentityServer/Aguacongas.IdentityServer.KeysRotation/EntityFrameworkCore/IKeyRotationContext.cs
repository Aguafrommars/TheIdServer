// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/EntityFrameworkCore/src/IDataProtectionKeyContext.cs
// with :
// namespace change from original Microsoft.AspNetCore.DataProtection.EntityFrameworkCore
// interface name change from original IDataProtectionKeyContext
// DbSet generic type change from original DataProtectionKey
using Microsoft.EntityFrameworkCore;

namespace Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore
{
    /// <summary>
    /// Interface used to store instances of <see cref="KeyRotationKey"/> in a <see cref="DbContext"/>
    /// </summary>
    public interface IKeyRotationContext // interface name change from original IDataProtectionKeyContext
    {
        /// <summary>
        /// A collection of <see cref="DataProtectionKey"/>
        /// </summary>
        DbSet<KeyRotationKey> KeyRotationKeys { get; } // DbSet generic type change from original DataProtectionKey
    }
}
