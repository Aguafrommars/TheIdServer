namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Options for aspects of the user interface.
    /// </summary>
    public class UserInteractionOptions
    {
        /// <summary>
        /// Gets or sets the login URL. If a local URL, the value must start with a leading slash.
        /// </summary>
        /// <value>
        /// The login URL.
        /// </value>
        public string LoginUrl { get; set; } = "/account/login";

        /// <summary>
        /// Gets or sets the login return URL parameter.
        /// </summary>
        /// <value>
        /// The login return URL parameter.
        /// </value>
        public string LoginReturnUrlParameter { get; set; } = "returnUrl";

        /// <summary>
        /// Gets or sets the logout URL. If a local URL, the value must start with a leading slash.
        /// </summary>
        /// <value>
        /// The logout URL.
        /// </value>
        public string LogoutUrl { get; set; } = "/account/logout";

        /// <summary>
        /// Gets or sets the logout identifier parameter.
        /// </summary>
        /// <value>
        /// The logout identifier parameter.
        /// </value>
        public string LogoutIdParameter { get; set; } = "logout";

        /// <summary>
        /// Gets or sets the consent URL. If a local URL, the value must start with a leading slash.
        /// </summary>
        /// <value>
        /// The consent URL.
        /// </value>
        public string ConsentUrl { get; set; } = "/consent";

        /// <summary>
        /// Gets or sets the consent return URL parameter.
        /// </summary>
        /// <value>
        /// The consent return URL parameter.
        /// </value>
        public string ConsentReturnUrlParameter { get; set; } = "returnUrl";

        /// <summary>
        /// Gets or sets the error URL. If a local URL, the value must start with a leading slash.
        /// </summary>
        /// <value>
        /// The error URL.
        /// </value>
        public string ErrorUrl { get; set; } = "/home/error";

        /// <summary>
        /// Gets or sets the error identifier parameter.
        /// </summary>
        /// <value>
        /// The error identifier parameter.
        /// </value>
        public string ErrorIdParameter { get; set; } = "errorId";

        /// <summary>
        /// Gets or sets the custom redirect return URL parameter.
        /// </summary>
        /// <value>
        /// The custom redirect return URL parameter.
        /// </value>
        public string CustomRedirectReturnUrlParameter { get; set; } = "returnUrl";

        /// <summary>
        /// Gets or sets the cookie message threshold. This limits how many cookies are created, and older ones will be purged.
        /// </summary>
        /// <value>
        /// The cookie message threshold.
        /// </value>
        public int CookieMessageThreshold { get; set; } = 2;

        /// <summary>
        /// Gets or sets the device verification URL.  If a local URL, the value must start with a leading slash.
        /// </summary>
        /// <value>
        /// The device verification URL.
        /// </value>
        public string DeviceVerificationUrl { get; set; } = "/device";

        /// <summary>
        /// Gets or sets the device verification user code paramater.
        /// </summary>
        /// <value>
        /// The device verification user code parameter.
        /// </value>
        public string DeviceVerificationUserCodeParameter { get; set; } = "userCode";

        /// <summary>
        /// Flag that allows return URL validation to accept full URL that includes the IdentityServer origin. Defaults to false.
        /// </summary>
        public bool AllowOriginInReturnUrl { get; set; }
    }
}