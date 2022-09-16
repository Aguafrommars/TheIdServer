using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ProviderOptions
    {
        /// <summary>
        /// Gets or sets the authority of the OpenID Connect (OIDC) identity provider.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the metadata URL of the OpenID Connect (OIDC) provider.
        /// </summary>
        public string MetadataUrl { get; set; }

        /// <summary>
        /// Gets or sets the client of the application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the list of scopes to request when signing in.
        /// </summary>
        /// <value>Defaults to <c>openid</c> and <c>profile</c>.</value>
        public IList<string> DefaultScopes { get; set; } = new List<string> { "openid", "profile" };

        /// <summary>
        /// Gets or sets the redirect URI for the application. The application will be redirected here after the user has completed the sign in
        /// process from the identity provider.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the post logout redirect URI for the application. The application will be redirected here after the user has completed the sign out
        /// process from the identity provider.
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the response type to use on the authorization flow. The valid values are specified by the identity provider metadata.
        /// </summary>
        public string ResponseType { get; set; }

        /// <summary>
        /// Gets or sets the response mode to use in the authorization flow.
        /// </summary>
        public string ResponseMode { get; set; }

        /// <summary>
        /// Gets or sets the additional provider parameters to use on the authorization flow.
        /// </summary>
        /// <remarks>
        /// These parameters are for the IdP and not for the application. Using those parameters in the application in any way on the login callback will likely introduce security issues as they should be treated as untrusted input.
        /// </remarks>
        public IDictionary<string, string> AdditionalProviderParameters { get; set; } = new Dictionary<string, string>();
    }
}
