// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Obsolete")]
    public class RegistrationException : Exception
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public RegistrationException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
