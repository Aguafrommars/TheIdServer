// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications Copyright (c) 2021 @Olivier Lefebvre

// This with is a copy of https://github.com/dotnet/aspnetcore/blob//v3.1.8/src/DataProtection/DataProtection/src/KeyManagement/Internal/ICacheableKeyRingProvider.cs
// with:
// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement.Internal
// add IKeyRingProvider interface derivation 
// add KeyManager property
// add RefreshCurrentKeyRing method

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;

// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement.Internal
namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ICacheableKeyRingProvider : IKeyRingProvider // add IKeyRingProvider interface derivation from oginal file
    {
        IKeyManager KeyManager { get; } // add KeyManager property from original file
        IKeyRing RefreshCurrentKeyRing(); // add RefreshCurrentKeyRing method from original file
        CacheableKeyRing GetCacheableKeyRing(DateTimeOffset now);
    }
}
