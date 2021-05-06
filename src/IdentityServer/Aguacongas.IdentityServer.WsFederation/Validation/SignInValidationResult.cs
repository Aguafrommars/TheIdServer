// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Stores;
using IdentityServer4.Models;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.WsFederation.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class SignInValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether this instance is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError => !string.IsNullOrWhiteSpace(Error);
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the ws federation message.
        /// </summary>
        /// <value>
        /// The ws federation message.
        /// </value>
        public WsFederationMessage WsFederationMessage { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public ClaimsPrincipal User { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [sign in required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sign in required]; otherwise, <c>false</c>.
        /// </value>
        public bool SignInRequired { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public Client Client { get; set; }
        /// <summary>
        /// Gets or sets the relying party.
        /// </summary>
        /// <value>
        /// The relying party.
        /// </value>
        public RelyingParty RelyingParty { get; set; }

        /// <summary>
        /// Gets or sets the reply URL.
        /// </summary>
        /// <value>
        /// The reply URL.
        /// </value>
        public string ReplyUrl { get; set; }
    }
}
