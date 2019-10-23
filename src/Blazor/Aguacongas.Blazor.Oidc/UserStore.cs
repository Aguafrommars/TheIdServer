using System.Security.Claims;

namespace Aguacongas.Blazor.Oidc
{
    public class UserStore
    {
        public ClaimsPrincipal User { get; internal set; }
    }
}
