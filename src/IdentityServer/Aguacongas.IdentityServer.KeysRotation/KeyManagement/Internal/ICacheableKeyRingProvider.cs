// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/KeyManagement/Internal/ICacheableKeyRingProvider.cs
// but adapted for our needs

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
