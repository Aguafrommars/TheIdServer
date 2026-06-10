// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation.Validation;

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<SignInValidationResult> ValidateAsync(WsFederationMessage message, ClaimsPrincipal user, CancellationToken cancellationToken);
}