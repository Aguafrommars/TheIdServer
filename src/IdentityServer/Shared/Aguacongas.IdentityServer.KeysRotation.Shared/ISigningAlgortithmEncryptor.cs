// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Models;
#else
using IdentityServer4.Models;
#endif
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ISigningAlgortithmEncryptor : IAuthenticatedEncryptor
    {
        SecurityKeyInfo GetSecurityKeyInfo(string signingAlgorithm);
        SigningCredentials GetSigningCredentials(string signingAlgorithm);
    }
}