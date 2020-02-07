using System;
using System.Runtime.Serialization;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ProblemException: Exception
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

        protected ProblemException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
