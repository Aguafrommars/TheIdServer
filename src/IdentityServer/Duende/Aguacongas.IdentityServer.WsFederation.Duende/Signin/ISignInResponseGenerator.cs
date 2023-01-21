// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Validation;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISignInResponseGenerator
    {
        /// <summary>
        /// Generates the response asynchronous.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns></returns>
        Task<WsFederationMessage> GenerateResponseAsync(SignInValidationResult validationResult);
    }
}