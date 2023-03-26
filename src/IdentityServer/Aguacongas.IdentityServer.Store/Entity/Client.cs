// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define a client appliction
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class Client : IAuditable, ICloneable<Client>, ILocalizable<ClientLocalizedResource>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow offline access].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow offline access]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowOfflineAccess { get; set; }

        /// <summary>
        /// Gets or sets the identity token lifetime.
        /// </summary>
        /// <value>
        /// The identity token lifetime.
        /// </value>
        public int IdentityTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets the access token lifetime.
        /// </summary>
        /// <value>
        /// The access token lifetime.
        /// </value>
        public int AccessTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets the authorization code lifetime.
        /// </summary>
        /// <value>
        /// The authorization code lifetime.
        /// </value>
        public int AuthorizationCodeLifetime { get; set; }

        /// <summary>
        /// Gets or sets the absolute refresh token lifetime.
        /// </summary>
        /// <value>
        /// The absolute refresh token lifetime.
        /// </value>
        public int AbsoluteRefreshTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets the sliding refresh token lifetime.
        /// </summary>
        /// <value>
        /// The sliding refresh token lifetime.
        /// </value>
        public int SlidingRefreshTokenLifetime { get; set; }

        /// <summary>
        /// Gets or sets the consent lifetime.
        /// </summary>
        /// <value>
        /// The consent lifetime.
        /// </value>
        public int? ConsentLifetime { get; set; }

        /// <summary>
        /// Gets or sets the refresh token usage.
        /// </summary>
        /// <value>
        /// The refresh token usage.
        /// </value>
        public int RefreshTokenUsage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [update access token claims on refresh].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [update access token claims on refresh]; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        /// <summary>
        /// Gets or sets the refresh token expiration.
        /// </summary>
        /// <value>
        /// The refresh token expiration.
        /// </value>
        public int RefreshTokenExpiration { get; set; }

        /// <summary>
        /// Gets or sets the type of the access token.
        /// </summary>
        /// <value>
        /// The type of the access token.
        /// </value>
        public int AccessTokenType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable local login].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable local login]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLocalLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include JWT identifier].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include JWT identifier]; otherwise, <c>false</c>.
        /// </value>        
        public bool IncludeJwtId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always send client claims].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [always send client claims]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysSendClientClaims { get; set; }

        /// <summary>
        /// Gets or sets the client claims prefix.
        /// </summary>
        /// <value>
        /// The client claims prefix.
        /// </value>
        [MaxLength(250)]
        public string ClientClaimsPrefix { get; set; }

        /// <summary>
        /// Gets or sets the pair wise subject salt.
        /// </summary>
        /// <value>
        /// The pair wise subject salt.
        /// </value>
        [MaxLength(200)]
        public string PairWiseSubjectSalt { get; set; }

        /// <summary>
        /// Gets or sets the user sso lifetime.
        /// </summary>
        /// <value>
        /// The user sso lifetime.
        /// </value>
        public int? UserSsoLifetime { get; set; }

        /// <summary>
        /// Gets or sets the type of the user code.
        /// </summary>
        /// <value>
        /// The type of the user code.
        /// </value>
        [MaxLength(100)]
        public string UserCodeType { get; set; }

        /// <summary>
        /// Gets or sets the device code lifetime.
        /// </summary>
        /// <value>
        /// The device code lifetime.
        /// </value>
        public int DeviceCodeLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether /[always include user claims in identifier token].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [always include user claims in identifier token]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [back channel logout session required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [back channel logout session required]; otherwise, <c>false</c>.
        /// </value>
        public bool BackChannelLogoutSessionRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this identity resouce is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the type of the protocol.
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        [Required]
        [MaxLength(200)]
        public string ProtocolType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require client secret].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require client secret]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        [MaxLength(200)]
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the client URI.
        /// </summary>
        /// <value>
        /// The client URI.
        /// </value>
        [MaxLength(2000)]
        [Url]
        public string ClientUri { get; set; }

        /// <summary>
        /// Gets or sets the logo URI.
        /// </summary>
        /// <value>
        /// The logo URI.
        /// </value>
        [MaxLength(2000)]
        [Url]
        public string LogoUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require consent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require consent]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require pkce].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require pkce]; otherwise, <c>false</c>.
        /// </value>
        public bool RequirePkce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow plain text pkce].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow plain text pkce]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowPlainTextPkce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow access tokens via browser].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow access tokens via browser]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAccessTokensViaBrowser { get; set; }

        /// <summary>
        /// Gets or sets the front channel logout URI.
        /// </summary>
        /// <value>
        /// The front channel logout URI.
        /// </value>
        [MaxLength(2000)]
        [Url]
        public string FrontChannelLogoutUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether /[front channel logout session required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front channel logout session required]; otherwise, <c>false</c>.
        /// </value>
        public bool FrontChannelLogoutSessionRequired { get; set; }

        /// <summary>
        /// Gets or sets the back channel logout URI.
        /// </summary>
        /// <value>
        /// The back channel logout URI.
        /// </value>
        [MaxLength(2000)]
        [Url]
        public string BackChannelLogoutUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow remember consent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow remember consent]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowRememberConsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [non editable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [non editable]; otherwise, <c>false</c>.
        /// </value>
        public bool NonEditable { get; set; }

        /// <summary>
        /// Gets or sets the policy URI.
        /// </summary>
        /// <value>
        /// The policy URI.
        /// </value>
        public string PolicyUri { get; set; }
        /// <summary>
        /// Gets or sets the tos URI.
        /// </summary>
        /// <value>
        /// The tos URI.
        /// </value>
        public string TosUri { get; set; }
        /// <summary>
        /// Gets or sets the registration token.
        /// </summary>
        /// <value>
        /// The registration token.
        /// </value>
        public Guid? RegistrationToken { get; set; }

        /// <summary>
        /// Gets or sets the relying party identifier.
        /// </summary>
        /// <value>
        /// The relying party identifier.
        /// </value>
        public string RelyingPartyId { get; set; }

        /// <summary>
        /// Gets or sets the backchannel authentication request lifetime in seconds.
        /// </summary>
        public int? CibaLifetime { get; set; }

        /// <summary>
        /// Gets or sets the backchannel polling interval in seconds.
        /// </summary>
        public int? PollingInterval { get; set; }

        /// <summary>
        /// When enabled, the client's token lifetimes (e.g. refresh tokens) will be tied to the user's session lifetime.
        /// This means when the user logs out, any revokable tokens will be removed.
        /// If using server-side sessions, expired sessions will also remove any revokable tokens, and backchannel logout will be triggered.
        /// This client's setting overrides the global CoordinateTokensWithUserSession configuration setting.
        /// </summary>
        public bool? CoordinateLifetimeWithUserSession { get; set; }

        /// <summary>
        /// Specifies whether the client must use a request object on authorize requests (defaults to <c>false</c>.)
        /// </summary>
        public bool RequireRequestObject { get; set; }

        /// <summary>
        /// Saml2P metadata
        /// </summary>
        public string Saml2PMetadata { get; set; }

        /// <summary>
        /// User acs artifact
        /// </summary>
        public bool UseAcsArtifact { get; set; }

        /// <summary>
        /// Signature validation certificate
        /// </summary>
        public byte[] SignatureValidationCertificate { get; set; }

        /// <summary>
        /// Gets or sets the relying.
        /// </summary>
        /// <value>
        /// The relying.
        /// </value>
        public virtual RelyingParty RelyingParty { get; set; }

        /// <summary>
        /// Gets or sets the identity provider restrictions.
        /// </summary>
        /// <value>
        /// The identity provider restrictions.
        /// </value>
        public virtual ICollection<ClientIdpRestriction> IdentityProviderRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        public virtual ICollection<ClientClaim> ClientClaims { get; set; }

        /// <summary>
        /// Gets or sets the client secrets.
        /// </summary>
        /// <value>
        /// The client secrets.
        /// </value>
        public virtual ICollection<ClientSecret> ClientSecrets { get; set; }

        /// <summary>
        /// Gets or sets the allowed grant types.
        /// </summary>
        /// <value>
        /// The allowed grant types.
        /// </value>
        public virtual ICollection<ClientGrantType> AllowedGrantTypes { get; set; }

        /// <summary>
        /// Gets or sets the redirect uris.
        /// </summary>
        /// <value>
        /// The redirect uris.
        /// </value>
        public virtual ICollection<ClientUri> RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the allowed scopes.
        /// </summary>
        /// <value>
        /// The allowed scopes.
        /// </value>
        public virtual ICollection<ClientScope> AllowedScopes { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public virtual ICollection<ClientProperty> Properties { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public virtual ICollection<ClientLocalizedResource> Resources { get; set; }

        /// <summary>
        /// Signing algorithm for identity token. If empty, will use the server default signing algorithm.
        /// </summary>
        public virtual ICollection<ClientAllowedIdentityTokenSigningAlgorithm> AllowedIdentityTokenSigningAlgorithms { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the modified at.
        /// </summary>
        /// <value>
        /// The modified at.
        /// </value>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Client Clone()
        {
            return MemberwiseClone() as Client;
        }
    }
}
