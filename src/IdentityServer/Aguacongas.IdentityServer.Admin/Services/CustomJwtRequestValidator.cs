// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

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
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">tokenValidationOptions</exception>
        public CustomJwtRequestValidator(IOptions<TokenValidationParameters> tokenValidationOptions, IHttpContextAccessor contextAccessor, IdentityServer4.Configuration.IdentityServerOptions options, ILogger<JwtRequestValidator> logger) : base(contextAccessor, options, logger)
        {
            _tokenValidationOptions = tokenValidationOptions?.Value ?? throw new ArgumentNullException(nameof(tokenValidationOptions));
        }

        /// <summary>
        /// Validates the JWT token
        /// </summary>
        /// <param name="jwtTokenString">JWT as a string</param>
        /// <param name="keys">The keys</param>
        /// <param name="client">The client</param>
        /// <returns></returns>
        protected override Task<JwtSecurityToken> ValidateJwtAsync(string jwtTokenString, IEnumerable<SecurityKey> keys, Client client)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = keys,
                ValidIssuer = client.ClientId,
                ValidAudience = AudienceUri,

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

            Handler.ValidateToken(jwtTokenString, tokenValidationParameters, out var token);

            return Task.FromResult((JwtSecurityToken)token);
        }
    }
}
