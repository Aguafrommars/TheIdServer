// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Duende.IdentityServer.Quickstart.UI
{
    /// <summary>
    /// Trowed when user click on malicious link
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Obsolete")]
    public class InvalidReturnUrlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidReturnUrlException"/> class.
        /// </summary>
        public InvalidReturnUrlException(): this("invalid return URL")
        {
        }

        public InvalidReturnUrlException(string message) : this(message, null)
        {
        }

        public InvalidReturnUrlException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
