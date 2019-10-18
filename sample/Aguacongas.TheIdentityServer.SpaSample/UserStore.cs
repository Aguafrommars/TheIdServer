using System.Security.Claims;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class UserStore
    {
        public ClaimsPrincipal User { get; internal set; }
    }
}
