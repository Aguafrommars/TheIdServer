// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Stores;
#if DUENDE
using Duende.IdentityServer;
using Duende.IdentityServer.Stores;
#else
using IdentityServer4;
using IdentityServer4.Stores;
#endif
using Microsoft.IdentityModel.Protocols.WsFederation;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class SignInValidator : ISignInValidator
    {
        private readonly IClientStore _clients;
        private readonly IRelyingPartyStore _relyingParties;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInValidator"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        /// <param name="relyingParties">The relying parties.</param>
        public SignInValidator(IClientStore clients, IRelyingPartyStore relyingParties)
        {
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _relyingParties = relyingParties ?? throw new ArgumentNullException(nameof(relyingParties));
        }

        /// <summary>
        /// Validates the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<SignInValidationResult> ValidateAsync(WsFederationMessage message, ClaimsPrincipal user)
        {
            var result = new SignInValidationResult
            {
                WsFederationMessage = message
            };

            // check client
            var client = await _clients.FindEnabledClientByIdAsync(message.Wtrealm);

            if (client == null)
            {
                return new SignInValidationResult
                {
                    Error = "invalid_relying_party",
                    ErrorMessage = $"{message.Wtrealm} client not found."
                };
            }
            if (client.ProtocolType != IdentityServerConstants.ProtocolTypes.WsFederation)
            {
                return new SignInValidationResult
                {
                    Error = "invalid_relying_party",
                    ErrorMessage = $"{message.Wtrealm} client is not a wsfed client."
                };
            }

            var rp = await _relyingParties.FindRelyingPartyByRealm(message.Wtrealm).ConfigureAwait(false);
            if (rp == null)
            {
                return new SignInValidationResult
                {
                    Error = "invalid_relying_party",
                    ErrorMessage = $"{message.Wtrealm} relying party not found."
                };
            }

            var replyUrl = message.Wreply ?? client.RedirectUris.FirstOrDefault();
            if (!Uri.TryCreate(replyUrl, UriKind.Absolute, out Uri _))
            {
                return new SignInValidationResult
                {
                    Error = "invalid_relying_party",
                    ErrorMessage = $"'{replyUrl}' is not a valid absolute uri. Message wreply received: {message.Wreply}. Client 1st redirect uri: {client.RedirectUris.FirstOrDefault()}."
                };
            }

            result.Client = client;
            result.ReplyUrl = replyUrl;
            result.RelyingParty = rp;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                result.SignInRequired = true;
            }

            result.User = user;

            return result;
        }
    }
}
