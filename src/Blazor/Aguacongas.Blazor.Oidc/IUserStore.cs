using System.Security.Claims;

namespace Aguacongas.Blazor.Oidc
{
    public interface IUserStore
    {
        string AccessToken { get; set; }
        ClaimsPrincipal User { get; set; }
        string AuthenticationScheme { get; set; }
    }
}