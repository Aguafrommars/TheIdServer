// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.IdentityServer
{
    [Serializable]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Obsolete")]
    public class IdentityException: Exception
    {
        public IEnumerable<IdentityError> Errors { get; set; }
        public IdentityException(string message) : base(message)
        {
        }

        public IdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IdentityException()
        {
        }
    }
}
