// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aguacongas.IdentityServer
{
    [Serializable]
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

        protected IdentityException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
