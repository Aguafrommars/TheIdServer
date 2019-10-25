namespace Aguacongas.Blazor.Oidc
{
    public class AuthorizationOptions
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public string NameClaimType { get; set; } = "name";

        public string RoleClaimType { get; set; } = "role";

        public string StoragePrefix { get; set; } = "oidc.";

        internal string VerifierStorageKey => $"{StoragePrefix}verifier";

        internal string ClaimsStorageKey => $"{StoragePrefix}claims";

        internal string TokensStorageKey => $"{StoragePrefix}tokens";

        internal string ExpireAtStorageKey => $"{StoragePrefix}expireAt";
    }
}
