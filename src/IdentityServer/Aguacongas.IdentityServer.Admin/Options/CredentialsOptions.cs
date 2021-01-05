// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.IdentityModel.Tokens;

namespace Aguacongas.IdentityServer.Admin.Options
{
    /// <summary>
    /// Options for credentials
    /// </summary>
    public class CredentialsOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="SigningCredentials"/> to use for signing tokens.
        /// </summary>
        public SigningCredentials SigningCredential { get; set; }
    }
}
