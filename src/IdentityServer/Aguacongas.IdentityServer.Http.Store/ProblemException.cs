using System;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
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
    }
}
