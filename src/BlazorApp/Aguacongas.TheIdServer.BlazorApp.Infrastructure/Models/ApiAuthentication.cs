using IdentityModel.Client;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Specifies how the client will transmit client ID and secret
    /// </summary>
    public enum ClientCredentialStyle
    {
        /// <summary>
        /// HTTP basic authentication
        /// </summary>
        AuthorizationHeader,
        /// <summary>
        /// Post values in body
        /// </summary>
        PostBody
    }
    /// <summary>
    /// Enum for specifying then encoding style of the basic authentication header
    /// </summary>
    public enum BasicAuthenticationHeaderStyle
    {
        /// <summary>
        ///     Recommended. Uses the encoding as described in the OAuth 2.0 spec (https://tools.ietf.org/html/rfc6749#section-2.3.1).
        ///     Base64(urlformencode(client_id) + ":" + urlformencode(client_secret))
        /// </summary>
        Rfc6749,
        /// <summary>
        ///     Uses the encoding as described in the original basic authentication spec (https://tools.ietf.org/html/rfc2617#section-2
        ///     - used by some non-OAuth 2.0 compliant authorization servers). Base64(client_id
        ///     + ":" + client_secret).
        /// </summary>
        Rfc2617
    }

    /// <summary>
    /// Options class provides information needed to control Bearer Authentication handler
    /// behavior
    /// </summary>
    public class ApiAuthentication
    {
        /// <summary>
        /// Gets or sets the api name. This is the audience
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the api secret. 
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Sets the URL of the introspection endpoint. If set, Authority is ignored
        /// </summary>
        public string IntrospectionEndpoint { get; set; }

        /// <summary>
        /// Specifies how client id and secret are being sent
        /// </summary>
        public ClientCredentialStyle ClientCredentialStyle { get; set; }

        /// <summary>
        /// Specifies how the authorization header gets formatted (if used)
        /// </summary>
        public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; }

        /// <summary>
        ///     Specifies the token type hint of the introspection client.
        /// </summary>
        public string TokenTypeHint { get; set; }

        /// <summary>
        ///     Specifies the claim type to use for the name claim (defaults to 'name')
        /// </summary>
        public string NameClaimType { get; set; }

        /// <summary>
        /// Specifies the claim type to use for the role claim (defaults to 'role')
        /// </summary>
        public string RoleClaimType { get; set; }


        /// <summary>
        ///     Specifies the authentication type to use for the authenticated identity. If not
        ///     set, the authentication scheme name is used as the authentication type (defaults
        ///     to null).
        /// </summary>
        public string AuthenticationType { get; set; }

        /// <summary>
        /// Specifies the policy for the discovery client
        /// </summary>
        public DiscoveryPolicy DiscoveryPolicy { get; set; }

        /// <summary>
        /// Specifies whether tokens that contain dots (most likely a JWT) are skipped
        /// </summary>
        public bool SkipTokensWithDots { get; set; }

        /// <summary>
        ///     Specifies whether the outcome of the token validation should be cached. This
        ///     reduces the load on the introspection endpoint at the STS
        /// </summary>
        public bool EnableCaching { get; set; }

        /// <summary>
        /// Specifies for how long the outcome of the token validation should be cached.
        /// </summary>
        public TimeSpan CacheDuration { get; set; }

        /// <summary>
        /// Specifies the prefix of the cache key (token).
        /// </summary>
        public string CacheKeyPrefix { get; set; }

        /// <summary>
        /// Gets or sets if HTTPS is required for the metadata address or authority. The
        /// default is true. This should be disabled only in development environments.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// Gets or sets the discovery endpoint for obtaining metadata
        /// </summary>
        public string MetadataAddress { get; set; }

        /// <summary>
        /// Gets or sets the Authority to use when making OpenIdConnect calls.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; }

        /// <summary>
        /// Gets or sets the timeout when using the backchannel to make an http call.
        /// </summary>
        public TimeSpan BackchannelTimeout { get; set; }

        /// <summary>
        /// Configuration provided directly by the developer. If provided, then MetadataAddress
        ///     and the Backchannel properties will not be used. This information should not
        ///     be updated during request processing.
        /// </summary>
        public OpenIdConnectConfiguration Configuration { get; set; }

        /// <summary>
        ///     Gets or sets if a metadata refresh should be attempted after a SecurityTokenSignatureKeyNotFoundException.
        ///     This allows for automatic recovery in the event of a signature key rollover.
        ///     This is enabled by default.
        /// </summary>
        public bool RefreshOnIssuerKeyNotFound { get; set; }


        /// <summary>
        ///     Defines whether the bearer token should be stored in the Microsoft.AspNetCore.Authentication.AuthenticationProperties
        ///     after a successful authorization.
        /// </summary>
        public bool SaveToken { get; set; }

        /// <summary>
        ///     Defines whether the token validation errors should be returned to the caller.
        ///     Enabled by default, this option can be disabled to prevent the JWT handler from
        ///     returning an error and an error_description in the WWW-Authenticate header.
        /// </summary>
        public bool IncludeErrorDetails { get; set; }


        /// <summary>
        ///     Gets or sets the Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions.MapInboundClaims
        ///     property on the default instance of System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
        ///     in SecurityTokenValidators, which is used when determining whether or not to
        ///     map claim types that are extracted when validating a System.IdentityModel.Tokens.Jwt.JwtSecurityToken.
        ///     If this is set to true, the Claim Type is set to the JSON claim 'name' after
        ///     translating using this mapping. Otherwise, no mapping occurs.
        ///     The default value is true.
        /// </summary>
        public bool MapInboundClaims { get; set; }

        /// <summary>
        ///     1 day is the default time interval that afterwards, Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions.ConfigurationManager
        ///     will obtain new configuration.
        /// </summary>
        public TimeSpan AutomaticRefreshInterval { get; set; }

        /// <summary>
        ///     The minimum time between Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions.ConfigurationManager
        ///     retrievals, in the event that a retrieval failed, or that a refresh was explicitly
        ///     requested. 30 seconds is the default.
        /// 
        /// </summary>
        public TimeSpan RefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the issuer that should be used for any claims that are created
        /// </summary>
        public string ClaimsIssuer { get; set; }

        /// <summary>
        ///     If set, this specifies a default scheme that authentication handlers should forward
        ///     all authentication operations to by default. The default forwarding logic will
        ///     check the most specific ForwardAuthenticate/Challenge/Forbid/SignIn/SignOut setting
        ///     first, followed by checking the ForwardDefaultSelector, followed by ForwardDefault.
        ///     The first non null result will be used as the target scheme to forward to.
        /// </summary>
        public string ForwardDefault { get; set; }

        /// <summary>
        ///     If set, this specifies the target scheme that this scheme should forward AuthenticateAsync
        ///     calls to. For example Context.AuthenticateAsync("ThisScheme") => Context.AuthenticateAsync("ForwardAuthenticateValue");
        ///     Set the target to the current scheme to disable forwarding and allow normal processing.
        /// </summary>
        public string ForwardAuthenticate { get; set; }

        /// <summary>
        ///     If set, this specifies the target scheme that this scheme should forward ChallengeAsync
        ///     calls to. For example Context.ChallengeAsync("ThisScheme") => Context.ChallengeAsync("ForwardChallengeValue");
        ///     Set the target to the current scheme to disable forwarding and allow normal processing.
        /// </summary>
        public string ForwardChallenge { get; set; }

        /// <summary>
        ///     If set, this specifies the target scheme that this scheme should forward ForbidAsync
        ///     calls to. For example Context.ForbidAsync("ThisScheme") => Context.ForbidAsync("ForwardForbidValue");
        ///     Set the target to the current scheme to disable forwarding and allow normal processing.
        /// </summary>
        public string ForwardForbid { get; set; }

        /// <summary>
        ///     If set, this specifies the target scheme that this scheme should forward SignInAsync
        ///     calls to. For example Context.SignInAsync("ThisScheme") => Context.SignInAsync("ForwardSignInValue");
        ///     Set the target to the current scheme to disable forwarding and allow normal processing.
        /// </summary>
        public string ForwardSignIn { get; set; }

        /// <summary>
        ///     If set, this specifies the target scheme that this scheme should forward SignOutAsync
        ///     calls to. For example Context.SignOutAsync("ThisScheme") => Context.SignOutAsync("ForwardSignOutValue");
        ///     Set the target to the current scheme to disable forwarding and allow normal processing.
        /// </summary>
        public string ForwardSignOut { get; set; }

    }
}