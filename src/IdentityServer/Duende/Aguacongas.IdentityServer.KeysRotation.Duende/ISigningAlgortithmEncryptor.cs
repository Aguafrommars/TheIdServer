// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.IdentityModel.Tokens;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ISigningAlgortithmEncryptor : IAuthenticatedEncryptor
    {
        SecurityKeyInfo GetSecurityKeyInfo(string signingAlgorithm);
        SigningCredentials GetSigningCredentials(string signingAlgorithm);
    }
}