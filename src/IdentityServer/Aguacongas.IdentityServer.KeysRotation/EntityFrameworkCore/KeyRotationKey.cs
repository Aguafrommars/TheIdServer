// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/EntityFrameworkCore/src/DataProtectionKey.cs
// with :
// namespace change from original Microsoft.AspNetCore.DataProtection.EntityFrameworkCore
// class name change from original DataProtectionKey
namespace Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore
{
    /// <summary>
    /// Code first model used by <see cref="EntityFrameworkCoreXmlRepository{TContext}"/>.
    /// </summary>
    public class KeyRotationKey // class name change from original DataProtectionKey
    {
        /// <summary>
        /// The entity identifier of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The XML representation of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string Xml { get; set; }
    }
}
