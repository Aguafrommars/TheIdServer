// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System;
using System.Runtime.Serialization;

namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected RegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
