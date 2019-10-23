using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.Blazor.Oidc
{
    public class AuthorizationOptions
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scopes { get; set; }

        public string VerifierStorageKey { get; set; } = "verfier";

        public string ClaimsStorageKey { get; set; } = "claims";

        public string TokensStorageKey { get; set; } = "tokens";

        public string ExpireAtStorageKey { get; set; } = "expireAt";
    }
}
