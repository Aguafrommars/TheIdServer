namespace Aguacongas.Blazor.Oidc
{
    public class AuthorizationOptions
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public string StoragePrefix { get; set; } = "oidc.";

        public string VerifierStorageKey => $"{StoragePrefix}verifier";

        public string ClaimsStorageKey => $"{StoragePrefix}claims";

        public string TokensStorageKey => $"{StoragePrefix}tokens";

        public string ExpireAtStorageKey => $"{StoragePrefix}expireAt";
    }
}
