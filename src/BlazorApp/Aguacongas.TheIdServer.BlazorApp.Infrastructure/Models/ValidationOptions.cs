using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// The ValidationOptions contains settings that affect some of the default validation behavior.
    /// </summary>
    public class ValidationOptions
    {
        /// <summary>
        ///  Collection of URI scheme prefixes that should never be used as custom URI schemes in the redirect_uri passed to tha authorize endpoint.
        /// </summary>
        public IEnumerable<string> InvalidRedirectUriPrefixes { get; set; } = new HashSet<string>
        {
            "javascript:",
            "file:",
            "data:",
            "mailto:",
            "ftp:",
            "blob:",
            "about:",
            "ssh:",
            "tel:",
            "view-source:",
            "ws:",
            "wss:"
        };
    }
}