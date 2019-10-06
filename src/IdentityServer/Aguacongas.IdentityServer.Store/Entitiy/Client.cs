using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class Client: Client<string>
    {

    }
    public class Client<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        //
        // Summary:
        //     Unique ID of the client
        public string ClientId { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether [allow offline access]. Defaults to false.
        public bool AllowOfflineAccess { get; set; }
        //
        // Summary:
        //     Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
        public int IdentityTokenLifetime { get; set; }
        //
        // Summary:
        //     Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
        public int AccessTokenLifetime { get; set; }
        //
        // Summary:
        //     Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
        public int AuthorizationCodeLifetime { get; set; }
        //
        // Summary:
        //     Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds /
        //     30 days
        public int AbsoluteRefreshTokenLifetime { get; set; }
        //
        // Summary:
        //     Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds /
        //     15 days
        public int SlidingRefreshTokenLifetime { get; set; }
        //
        // Summary:
        //     Lifetime of a user consent in seconds. Defaults to null (no expiration)
        public int? ConsentLifetime { get; set; }
        //
        // Summary:
        //     ReUse: the refresh token handle will stay the same when refreshing tokens OneTime:
        //     the refresh token handle will be updated when refreshing tokens
        public int RefreshTokenUsage { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether the access token (and its claims) should
        //     be updated on a refresh token request. Defaults to false.
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        //
        // Summary:
        //     Absolute: the refresh token will expire on a fixed point in time (specified by
        //     the AbsoluteRefreshTokenLifetime) Sliding: when refreshing the token, the lifetime
        //     of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime).
        //     The lifetime will not exceed AbsoluteRefreshTokenLifetime.
        public int RefreshTokenExpiration { get; set; }
        //
        // Summary:
        //     Specifies whether the access token is a reference token or a self contained JWT
        //     token (defaults to Jwt).
        public int AccessTokenType { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether the local login is allowed for this client.
        //     Defaults to true.
        public bool EnableLocalLogin { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether JWT access tokens should include an identifier.
        //     Defaults to false.
        public bool IncludeJwtId { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether client claims should be always included
        //     in the access tokens - or only for client credentials flow. Defaults to false
        public bool AlwaysSendClientClaims { get; set; }
        //
        // Summary:
        //     Gets or sets a value to prefix it on client claim types. Defaults to client_.
        [MaxLength(250)]
        public string ClientClaimsPrefix { get; set; }
        //
        // Summary:
        //     Gets or sets a salt value used in pair-wise subjectId generation for users of
        //     this client.
        [MaxLength(200)]
        public string PairWiseSubjectSalt { get; set; }
        //
        // Summary:
        //     The maximum duration (in seconds) since the last time the user authenticated.
        public int? UserSsoLifetime { get; set; }
        //
        // Summary:
        //     Gets or sets the type of the device flow user code.
        [MaxLength(100)]
        public string UserCodeType { get; set; }
        //
        // Summary:
        //     Gets or sets the device code lifetime.
        public int DeviceCodeLifetime { get; set; }
        //
        // Summary:
        //     When requesting both an id token and access token, should the user claims always
        //     be added to the id token instead of requring the client to use the userinfo endpoint.
        //     Defaults to false.
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        //
        // Summary:
        //     Specifies is the user's session id should be sent to the BackChannelLogoutUri.
        //     Defaults to true.
        public bool BackChannelLogoutSessionRequired { get; set; }
        //
        // Summary:
        //     Specifies if client is enabled (defaults to true)
        public bool Enabled { get; set; }
        //
        // Summary:
        //     Gets or sets the protocol type.
        [MaxLength(200)]
        public string ProtocolType { get; set; }
        //
        // Summary:
        //     If set to false, no client secret is needed to request tokens at the token endpoint
        //     (defaults to true)
        public bool RequireClientSecret { get; set; }
        //
        // Summary:
        //     Client display name (used for logging and consent screen)
        [MaxLength(200)]
        public string ClientName { get; set; }
        //
        // Summary:
        //     Description of the client.
        [MaxLength(1000)]
        public string Description { get; set; }
        //
        // Summary:
        //     URI to further information about client (used on consent screen)
        [MaxLength(2000)]
        public string ClientUri { get; set; }
        //
        // Summary:
        //     URI to client logo (used on consent screen)
        [MaxLength(2000)]
        public string LogoUri { get; set; }
        //
        // Summary:
        //     Specifies whether a consent screen is required (defaults to true)
        public bool RequireConsent { get; set; }
        //
        // Summary:
        //     Specifies whether a proof key is required for authorization code based token
        //     requests (defaults to false).
        public bool RequirePkce { get; set; }
        //
        // Summary:
        //     Specifies whether a proof key can be sent using plain method (not recommended
        //     and defaults to false.)
        public bool AllowPlainTextPkce { get; set; }
        //
        // Summary:
        //     Controls whether access tokens are transmitted via the browser for this client
        //     (defaults to false). This can prevent accidental leakage of access tokens when
        //     multiple response types are allowed.
        public bool AllowAccessTokensViaBrowser { get; set; }
        //
        // Summary:
        //     Specifies logout URI at client for HTTP front-channel based logout.
        [MaxLength(2000)]
        public string FrontChannelLogoutUri { get; set; }
        //
        // Summary:
        //     Specifies is the user's session id should be sent to the FrontChannelLogoutUri.
        //     Defaults to true.
        public bool FrontChannelLogoutSessionRequired { get; set; }
        //
        // Summary:
        //     Specifies logout URI at client for HTTP back-channel based logout.
        [MaxLength(2000)]
        public string BackChannelLogoutUri { get; set; }
        //
        // Summary:
        //     Specifies whether user can choose to store consent decisions (defaults to true)
        public bool AllowRememberConsent { get; set; }

        //
        // Summary:
        //     Specifies which external IdPs can be used with this client (if list is empty
        //     all IdPs are allowed). Defaults to empty.
        public virtual ICollection<ClientIdPRestriction<TKey>> IdentityProviderRestrictions { get; set; }
        //
        // Summary:
        //     Allows settings claims for the client (will be included in the access token).
        public virtual ICollection<ClientClaim<TKey>> Claims { get; set; }
        //
        // Summary:
        //     Client secrets - only relevant for flows that require a secret
        public virtual ICollection<ClientSecret<TKey>> ClientSecrets { get; set; }
        //
        // Summary:
        //     Gets or sets the allowed CORS origins for JavaScript clients.
        public virtual ICollection<ClientCorsOrigin<TKey>> AllowedCorsOrigins { get; set; }
        //
        // Summary:
        //     Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit,
        //     Hybrid, ResourceOwner, ClientCredentials).
        public virtual ICollection<ClientGrantType<TKey>> AllowedGrantTypes { get; set; }
        //
        // Summary:
        //     Specifies allowed URIs to return tokens or authorization codes to
        public virtual ICollection<ClientRedirectUri<TKey>> RedirectUris { get; set; }
        //
        // Summary:
        //     Specifies allowed URIs to redirect to after logout
        public virtual ICollection<ClientPostLogoutRedirectUri<TKey>> PostLogoutRedirectUris { get; set; }
        //
        // Summary:
        //     Specifies the api scopes that the client is allowed to request. If empty, the
        //     client can't access any scope
        public virtual ICollection<ClientScope<TKey>> AllowedScopes { get; set; }
        //
        // Summary:
        //     Gets or sets the custom properties for the client.
        public virtual ICollection<ClientProperty<TKey>> Properties { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
