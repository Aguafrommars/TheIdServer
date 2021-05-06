// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISignInValidator
    {
        /// <summary>
        /// Validates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task<SignInValidationResult> ValidateAsync(WsFederationMessage message, ClaimsPrincipal user);
    }
}