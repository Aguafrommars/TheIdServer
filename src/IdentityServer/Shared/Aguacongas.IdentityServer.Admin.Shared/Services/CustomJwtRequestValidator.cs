// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using IdentityModel;
#if DUENDE
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Duende.IdentityServer.Services;
#else
using IdentityServer4.Models;
using IdentityServer4.Validation;
#endif
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="JwtRequestValidator" />
    public class CustomJwtRequestValidator : JwtRequestValidator
    {
        private readonly TokenValidationParameters _tokenValidationOptions;


#pragma warning disable CS1587 // XML comment is not placed on a valid language element
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJwtRequestValidator" /> class.
        /// </summary>
        /// <param name="tokenValidationOptions">The token validation options.</param>
#if DUENDE
        /// <param name="issuerNameService">The issuer name service.</param>
#else
        /// <param name="contextAccessor">The context accessor.</param>
#endif
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">tokenValidationOptions</exception>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public CustomJwtRequestValidator(IOptions<TokenValidationParameters> tokenValidationOptions,
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
#if DUENDE
            Duende.IdentityServer.Configuration.IdentityServerOptions options,
            IIssuerNameService issuerNameService,
#else
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            IHttpContextAccessor contextAccessor,
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            IdentityServer4.Configuration.IdentityServerOptions options, 
#endif
            ILogger<JwtRequestValidator> logger) :
#if DUENDE
            base(options, issuerNameService, logger)
#else
            base(contextAccessor, options, logger)
#endif
        {
            _tokenValidationOptions = tokenValidationOptions?.Value ?? throw new ArgumentNullException(nameof(tokenValidationOptions));
        }

#if DUENDE
        /// <summary>
        /// Validates the JWT token
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        protected override async Task<JsonWebToken> ValidateJwtAsync(JwtRequestValidationContext context, IEnumerable<SecurityKey> keys)
#else
        /// <summary>
        /// Validates the JWT token
        /// </summary>
        /// <param name="jwtTokenString">JWT as a string</param>
        /// <param name="keys">The keys</param>
        /// <param name="client">The client</param>
        /// <returns></returns>
        protected override Task<JwtSecurityToken> ValidateJwtAsync(string jwtTokenString, IEnumerable<SecurityKey> keys, Client client)
#endif
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = keys,
#if DUENDE
                ValidIssuer = context.Client.ClientId,
                ValidAudience = await GetAudienceUri().ConfigureAwait(false),
#else
                ValidIssuer = client.ClientId,
                ValidAudience = AudienceUri,
#endif
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

#if DUENDE
            var result = Handler.ValidateToken(context.JwtTokenString, tokenValidationParameters);
            if (!result.IsValid)
            {
                throw result.Exception;
            }

            return (JsonWebToken)result.SecurityToken;
#else
            Handler.ValidateToken(jwtTokenString, tokenValidationParameters, out var token);

            return Task.FromResult((JwtSecurityToken)token);
#endif
        }
    }
}
