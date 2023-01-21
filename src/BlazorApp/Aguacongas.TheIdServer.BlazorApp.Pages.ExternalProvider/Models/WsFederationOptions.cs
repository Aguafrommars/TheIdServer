// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre

namespace Aguacongas.TheIdServer.BlazorApp.Models
{

    /// <summary>
    /// WS-Federation options
    /// </summary>
    /// <seealso cref="RemoteAuthenticationOptions" />
    public class WsFederationOptions : RemoteAuthenticationOptions
    {
        /// <summary>
        /// Requests received on this path will cause the handler to invoke SignOut using
        /// the SignOutScheme.
        /// </summary>
        /// <value>
        /// The remote sign out path.
        /// </value>
        public string RemoteSignOutPath { get; set; }

        /// <summary>
        ///     The Ws-Federation protocol allows the user to initiate logins without contacting
        ///     the application for a Challenge first. However, that flow is susceptible to XSRF
        ///     and other attacks so it is disabled here by default.        
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow unsolicited logins]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowUnsolicitedLogins { get; set; }

        /// <summary>
        /// Gets or sets if HTTPS is required for the metadata address or authority. The
        /// default is true. This should be disabled only in development environments.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require HTTPS metadata]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// Indicates that the authentication session lifetime (e.g. cookies) should match
        /// that of the authentication token. If the token does not provide lifetime information
        /// then normal session lifetimes will be used. This is enabled by default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use token lifetime]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTokenLifetime { get; set; }
        /// <summary>
        /// Gets or sets the 'wtrealm'.
        /// </summary>
        /// <value>
        /// The wtrealm.
        /// </value>
        public string Wtrealm { get; set; }

        /// <summary>
        /// Gets or sets the 'wreply' value used during sign-out. If none is specified then
        /// the value from the Wreply field is used.
        /// </summary>
        /// <value>
        /// The sign out wreply.
        /// </value>
        public string SignOutWreply { get; set; }

        /// <summary>
        /// Gets or sets the 'wreply'. CallbackPath must be set to match or cleared so it
        /// can be generated dynamically. This field is optional. If not set then it will
        /// be generated from the current request and the CallbackPath.
        /// </summary>
        /// <value>
        /// The wreply.
        /// </value>
        public string Wreply { get; set; }


        /// <summary>
        ///     Indicates if requests to the CallbackPath may also be for other components. If
        ///     enabled the handler will pass requests through that do not contain WsFederation
        ///     authentication responses. Disabling this and setting the CallbackPath to a dedicated
        ///     endpoint may provide better error handling. This is disabled by default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip unrecognized requests]; otherwise, <c>false</c>.
        /// </value>
        public bool SkipUnrecognizedRequests { get; set; }
        /// <summary>
        ///     Gets or sets if a metadata refresh should be attempted after a SecurityTokenSignatureKeyNotFoundException.
        ///     This allows for automatic recovery in the event of a signature key rollover.
        ///     This is enabled by default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [refresh on issuer key not found]; otherwise, <c>false</c>.
        /// </value>
        public bool RefreshOnIssuerKeyNotFound { get; set; }

        /// <summary>
        ///     Gets or sets the address to retrieve the wsFederation metadata
        /// </summary>
        /// <value>
        /// The metadata address.
        /// </value>
        public string MetadataAddress { get; set; }

        /// <summary>
        ///     The Authentication Scheme to use with SignOutAsync from RemoteSignOutPath. SignInScheme
        ///     will be used if this is not set.
        /// </summary>
        /// <value>
        /// The sign out scheme.
        /// </value>
        public string SignOutScheme { get; set; }
    }
}
