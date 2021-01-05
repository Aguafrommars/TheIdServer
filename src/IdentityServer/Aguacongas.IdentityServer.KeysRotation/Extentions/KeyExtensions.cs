// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/KeyManagement/KeyExtensions.cs
// with namespace change from proginal Microsoft.AspNetCore.DataProtection.KeyManagement

using System;

// namespace change from proginal Microsoft.AspNetCore.DataProtection.KeyManagement
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
