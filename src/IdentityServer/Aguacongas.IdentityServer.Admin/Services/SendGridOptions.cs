// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// SendGrid options
    /// </summary>
    public class SendGridOptions
    {
        /// <summary>
        /// Gets or sets the send grid user.
        /// </summary>
        /// <value>
        /// The send grid user.
        /// </value>
        public string SendGridUser { get; set; }

        /// <summary>
        /// Gets or sets the send grid key.
        /// </summary>
        /// <value>
        /// The send grid key.
        /// </value>
        public string SendGridKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable click tracking].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable click tracking]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableClickTracking { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; set; } = "no-reply@aguacongas.com";
    }
}