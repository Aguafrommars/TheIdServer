// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Obsolete")]
    public class ProblemException : Exception
    {
        public ProblemDetails Details { get; set; }

        public ProblemException(string message) : base(message)
        {
        }

        public ProblemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ProblemException()
        {
        }
    }
}
