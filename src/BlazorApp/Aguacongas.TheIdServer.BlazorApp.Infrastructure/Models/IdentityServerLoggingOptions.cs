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
        public IEnumerable<string> BackchannelAuthenticationRequestSensitiveValuesFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> TokenRequestSensitiveValuesFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> AuthorizeRequestSensitiveValuesFilter { get; set; }
    }
}