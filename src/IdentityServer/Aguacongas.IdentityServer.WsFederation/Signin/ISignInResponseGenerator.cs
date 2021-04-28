// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Validation;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    public interface ISignInResponseGenerator
    {
        Task<WsFederationMessage> GenerateResponseAsync(SignInValidationResult validationResult);
    }
}