using System;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityUserException: Exception
    {
        public IdentityUserException(string message) : base(message)
        {
        }

        public IdentityUserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
