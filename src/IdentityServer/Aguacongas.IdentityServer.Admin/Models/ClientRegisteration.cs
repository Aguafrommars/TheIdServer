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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("client_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

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
        /// Gets or sets the type of the subject.
        /// </summary>
        /// <value>
        /// The type of the subject.
        /// </value>
        [JsonProperty("subject_type")]
        public string SubjectType { get; set; }
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
