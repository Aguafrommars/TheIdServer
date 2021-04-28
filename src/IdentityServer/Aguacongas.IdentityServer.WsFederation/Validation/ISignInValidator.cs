// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation.Validation
{
    public interface ISignInValidator
    {
        Task<SignInValidationResult> ValidateAsync(WsFederationMessage message, ClaimsPrincipal user);
    }
}