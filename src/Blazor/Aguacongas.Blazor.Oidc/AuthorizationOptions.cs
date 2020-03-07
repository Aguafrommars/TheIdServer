namespace Aguacongas.TheIdServer.Blazor.Oidc
{
    public class AuthorizationOptions
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public string RedirectUri { get; set; }

        public string LogoutUri { get; set; } = "/account/logout";

        public string NameClaimType { get; set; } = "name";

        public string RoleClaimType { get; set; } = "role";

        public string StoragePrefix { get; set; } = "oidc.";

        public string BackUriStorageKey => $"{StoragePrefix}backUri";

        public string TokenEndpointStorageKey => $"{StoragePrefix}tokenEnpoint";

        public string UserInfoEndpointStorageKey => $"{StoragePrefix}userInfoEnpoint";

        public string VerifierStorageKey => $"{StoragePrefix}verifier";

        public string ClaimsStorageKey => $"{StoragePrefix}claims";

        public string TokensStorageKey => $"{StoragePrefix}tokens";

        public string ExpireAtStorageKey => $"{StoragePrefix}expireAt";

        public string RevocationEndpointStorageKey => $"{StoragePrefix}revocation";
    }
}
