using Aguacongas.IdentityServer.Admin.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// 
    /// </summary>
    [JsonConverter(typeof(ClientRegisterationConverter))]
    public class ClientRegisteration
    {
        /// <summary>
        /// Gets or sets the redirect uris.
        /// </summary>
        /// <value>
        /// The redirect uris.
        /// </value>
        [JsonProperty("redirect_uris")]
        [Required]
        public IEnumerable<string> RedirectUris { get; set; }

        /// <summary>
        /// Gets or sets the response types.
        /// </summary>
        /// <value>
        /// The response types.
        /// </value>
        [JsonProperty("response_types")]
        public IEnumerable<string> ResponseTypes { get; set; }

        /// <summary>
        /// Gets or sets the grant types.
        /// </summary>
        /// <value>
        /// The grant types.
        /// </value>
        [JsonProperty("grant_types")]
        public IEnumerable<string> GrantTypes { get; set; }

        /// <summary>
        /// Gets or sets the type of the application.
        /// </summary>
        /// <value>
        /// The type of the application.
        /// </value>
        [JsonProperty("application_type")]
        public string ApplicationType { get; set; } = "web";

        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>
        /// The contacts.
        /// </value>
        [JsonProperty("contacts")]
        public IEnumerable<string> Contacts { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        [JsonProperty("client_name")]
        public IEnumerable<LocalizableProperty> ClientNames { get; set; }

        /// <summary>
        /// Gets or sets the logo URI.
        /// </summary>
        /// <value>
        /// The logo URI.
        /// </value>
        [JsonProperty("logo_uri")]
        public IEnumerable<LocalizableProperty> LogoUris { get; set; }

        /// <summary>
        /// Gets or sets the client URI.
        /// </summary>
        /// <value>
        /// The client URI.
        /// </value>
        [JsonProperty("client_uri")]
        public IEnumerable<LocalizableProperty> ClientUris { get; set; }

        /// <summary>
        /// Gets or sets the policy URI.
        /// </summary>
        /// <value>
        /// The policy URI.
        /// </value>
        [JsonProperty("policy_uri")]
        public IEnumerable<LocalizableProperty> PolicyUris { get; set; }

        /// <summary>
        /// Gets or sets the tos URI.
        /// </summary>
        /// <value>
        /// The tos URI.
        /// </value>
        [JsonProperty("tos_uri")]
        public IEnumerable<LocalizableProperty> TosUris { get; set; }

        /// <summary>
        /// Gets or sets the JWKS URI.
        /// </summary>
        /// <value>
        /// The JWKS URI.
        /// </value>
        [JsonProperty("jwks_uri")]
        public string JwksUri { get; set; }

        /// <summary>
        /// Gets or sets the JWKS.
        /// </summary>
        /// <value>
        /// The JWKS.
        /// </value>
        [JsonProperty("jwks")]
        public string Jwks { get; set; }

        /// <summary>
        /// Gets or sets the sector identifier URI.
        /// </summary>
        /// <value>
        /// The sector identifier URI.
        /// </value>
        [JsonProperty("sector_identifier_uri")]
        public string SectorIdentifierUri { get; set; }

        /// <summary>
        /// Gets or sets the type of the subject.
        /// </summary>
        /// <value>
        /// The type of the subject.
        /// </value>
        [JsonProperty("subject_type")]
        public string SubjectType { get; set; }

        /// <summary>
        /// Gets or sets the identifier token signed response alg.
        /// </summary>
        /// <value>
        /// The identifier token signed response alg.
        /// </value>
        [JsonProperty("id_token_signed_response_alg")]
        public string IdTokenSignedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the identifier token encrypted response alg.
        /// </summary>
        /// <value>
        /// The identifier token encrypted response alg.
        /// </value>
        [JsonProperty("id_token_encrypted_response_alg")]
        public string IdTokenEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the identifier token encrypted response enc.
        /// </summary>
        /// <value>
        /// The identifier token encrypted response enc.
        /// </value>
        [JsonProperty("id_token_encrypted_response_enc")]
        public string IdTokenEncryptedResponseEnc { get; set; }

        /// <summary>
        /// Gets or sets the userinfo signed response alg.
        /// </summary>
        /// <value>
        /// The userinfo signed response alg.
        /// </value>
        [JsonProperty("userinfo_signed_response_alg")]
        public string UserinfoSignedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the userinfo encrypted response alg.
        /// </summary>
        /// <value>
        /// The userinfo encrypted response alg.
        /// </value>
        [JsonProperty("userinfo_encrypted_response_alg")]
        public string UserinfoEncryptedResponseAlg { get; set; }

        /// <summary>
        /// Gets or sets the userinfo encrypted response enc.
        /// </summary>
        /// <value>
        /// The userinfo encrypted response enc.
        /// </value>
        [JsonProperty("userinfo_encrypted_response_enc")]
        public string UserinfoEncryptedResponseEnc { get; set; }

        /// <summary>
        /// Gets or sets the request object signing alg.
        /// </summary>
        /// <value>
        /// The request object signing alg.
        /// </value>
        [JsonProperty("request_object_signing_alg")]
        public string RequestObjectSigningAlg { get; set; }

        /// <summary>
        /// Gets or sets the request object encryption alg.
        /// </summary>
        /// <value>
        /// The request object encryption alg.
        /// </value>
        [JsonProperty("request_object_encryption_alg")]
        public string RequestObjectEncryptionAlg { get; set; }

        /// <summary>
        /// Gets or sets the request object encryption enc.
        /// </summary>
        /// <value>
        /// The request object encryption enc.
        /// </value>
        [JsonProperty("request_object_encryption_enc")]
        public string RequestObjectEncryptionEnc { get; set; }

        /// <summary>
        /// Gets or sets the token endpoint authentication method.
        /// </summary>
        /// <value>
        /// The token endpoint authentication method.
        /// </value>
        [JsonProperty("token_endpoint_auth_method")]
        public string TokenEndpointAuthMethod { get; set; }

        /// <summary>
        /// Gets or sets the token endpoint authentication signing alg.
        /// </summary>
        /// <value>
        /// The token endpoint authentication signing alg.
        /// </value>
        [JsonProperty("token_endpoint_auth_signing_alg")]
        public string TokenEndpointAuthSigningAlg { get; set; }

        /// <summary>
        /// Gets or sets the default maximum age.
        /// </summary>
        /// <value>
        /// The default maximum age.
        /// </value>
        [JsonProperty("default_max_age")]
        public string DefaultMaxAge { get; set; }

        /// <summary>
        /// Gets or sets the require authentication time.
        /// </summary>
        /// <value>
        /// The require authentication time.
        /// </value>
        [JsonProperty("require_auth_time")]
        public string RequireAuthTime { get; set; }

        /// <summary>
        /// Gets or sets the default acr values.
        /// </summary>
        /// <value>
        /// The default acr values.
        /// </value>
        [JsonProperty("default_acr_values")]
        public string DefaultAcrValues { get; set; }

        /// <summary>
        /// Gets or sets the initiate login URI.
        /// </summary>
        /// <value>
        /// The initiate login URI.
        /// </value>
        [JsonProperty("initiate_login_uri")]
        public string InitiateLoginUri { get; set; }

        /// <summary>
        /// Gets or sets the request uris.
        /// </summary>
        /// <value>
        /// The request uris.
        /// </value>
        [JsonProperty("request_uris")]
        public IEnumerable<string> RequestUris { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ApplicationType
    {
        /// <summary>
        /// The native
        /// </summary>
        Native,
        /// <summary>
        /// The web
        /// </summary>
        Web
    }
}
