using System.Security.Claims;

namespace Aguacongas.TheIdServer.Blazor.Oidc
{
    public class UserStore : IUserStore
    {
        public ClaimsPrincipal User { get; set; }
        public string AccessToken { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
