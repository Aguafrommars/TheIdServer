// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using IsConfiguration = Duende.IdentityServer.Configuration;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="JwtRequestValidator" />
    public class CustomJwtRequestValidator : JwtRequestValidator
    {
        private readonly TokenValidationParameters _tokenValidationOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJwtRequestValidator" /> class.
        /// </summary>
        /// <param name="tokenValidationOptions">The token validation options.</param>
        /// <param name="issuerNameService">The issuer name service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">tokenValidationOptions</exception>
        public CustomJwtRequestValidator(IOptions<TokenValidationParameters> tokenValidationOptions,
            IsConfiguration.IdentityServerOptions options,
            IIssuerNameService issuerNameService,
            ILogger<JwtRequestValidator> logger) :
            base(options, issuerNameService, logger)
        {
            _tokenValidationOptions = tokenValidationOptions?.Value ?? throw new ArgumentNullException(nameof(tokenValidationOptions));
        }

        /// <summary>
        /// Validates the JWT token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        protected override async Task<JsonWebToken> ValidateJwtAsync(JwtRequestValidationContext context, IEnumerable<SecurityKey> keys)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = keys,
                ValidIssuer = context.Client.ClientId,
                ValidAudience = await GetAudienceUri().ConfigureAwait(false),
                ValidateIssuerSigningKey = _tokenValidationOptions.ValidateIssuerSigningKey,
                ValidateIssuer = _tokenValidationOptions.ValidateIssuer,                
                ValidateAudience = _tokenValidationOptions.ValidateAudience,
                ValidateLifetime = _tokenValidationOptions.ValidateLifetime,

                RequireAudience = _tokenValidationOptions.RequireAudience,
                RequireSignedTokens = _tokenValidationOptions.RequireSignedTokens,
                RequireExpirationTime = _tokenValidationOptions.RequireExpirationTime
            };

            if (Options.StrictJarValidation)
            {
                tokenValidationParameters.ValidTypes = new[] { JwtClaimTypes.JwtTypes.AuthorizationRequest };
            }

            var result = Handler.ValidateToken(context.JwtTokenString, tokenValidationParameters);
            if (!result.IsValid)
            {
                throw result.Exception;
            }

            return (JsonWebToken)result.SecurityToken;
        }
    }
}
