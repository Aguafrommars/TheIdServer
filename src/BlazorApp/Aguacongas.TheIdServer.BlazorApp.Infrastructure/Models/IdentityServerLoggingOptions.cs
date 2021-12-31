using IdentityModel;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Options for configuring logging behavior
    /// </summary>
    public class IdentityServerLoggingOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> BackchannelAuthenticationRequestSensitiveValuesFilter { get; set; } =
            new HashSet<string>
            {
                OidcConstants.TokenRequest.ClientSecret,
                OidcConstants.TokenRequest.ClientAssertion,
                OidcConstants.AuthorizeRequest.IdTokenHint
            };

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> TokenRequestSensitiveValuesFilter { get; set; } =
            new HashSet<string>
            {
                OidcConstants.TokenRequest.ClientSecret,
                OidcConstants.TokenRequest.Password,
                OidcConstants.TokenRequest.ClientAssertion,
                OidcConstants.TokenRequest.RefreshToken,
                OidcConstants.TokenRequest.DeviceCode
            };

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> AuthorizeRequestSensitiveValuesFilter { get; set; } =
            new HashSet<string>
            {
                OidcConstants.AuthorizeRequest.IdTokenHint
            };
    }
}