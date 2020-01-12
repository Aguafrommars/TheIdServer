using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Extensions
{
    public static class ClientExtensions
    {
        public static bool IsWebClient(this Client client)
        {
            return client.ProtocolType == "oidc" &&
                (client.AllowedGrantTypes.Any(g => g.GrantType == "authorization_code" ||
                    g.GrantType == "hybrid" ||
                    g.GrantType == "implicit" ||
                    g.GrantType == "urn:ietf:params:oauth:grant-type:device_code") ||
                client.HasCustomGrantType());
        }

        public static bool IsSpaClient(this Client client)
        {
            return client.AllowedGrantTypes.Any(g => g.GrantType == "authorization_code" ||
                    g.GrantType == "implicit") ||
                client.HasCustomGrantType();
        }

        public static bool IsAuthorizationCodeClient(this Client client)
        {
            return client.AllowedGrantTypes.Any(g => g.GrantType == "authorization_code") ||
                client.HasCustomGrantType();
        }

        public static bool CanHandlePostLogout(this Client client)
        {
            return client.AllowedGrantTypes.Any(g => g.GrantType == "authorization_code" ||
                    g.GrantType == "hybrid" ||
                    g.GrantType == "implicit") ||
                client.HasCustomGrantType();
        }

        public static bool CanUseRefreshToken(this Client client)
        {
            return (client.AllowedGrantTypes.Any(g => g.GrantType == "authorization_code" ||
                    g.GrantType == "hybrid" ||
                    g.GrantType == "password") ||
                client.HasCustomGrantType()) &&
                client.AllowOfflineAccess;
        }

        public static bool HasUser(this Client client)
        {
            return CanHandlePostLogout(client);
        }

        public static bool IsDevice(this Client client)
        {
            return client.AllowedGrantTypes.Any(g => g.GrantType == "urn:ietf:params:oauth:grant-type:device_code") ||
                client.HasCustomGrantType();
        }

        public static bool HasCustomGrantType(this Client client)
        {
            return !client.AllowedGrantTypes.Where(g => g.Id != null)
                .All(g => GrantTypes.Instance.ContainsKey(g.GrantType));
        }
    }
}
