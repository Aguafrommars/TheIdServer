using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authentication;

namespace Aguacongas.IdentityServer.Store
{
    public static class EntityExtensions
    {
        public static IdentityProvider ToIdentityProvider(this AuthenticationScheme scheme)
        {
            if (scheme == null)
            {
                return null;
            }
            return new IdentityProvider
            {
                Id = scheme.Name,
                DisplayName = scheme.DisplayName
            };
        }
    }
}
