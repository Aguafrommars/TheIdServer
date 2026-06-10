// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IsConfiguration = Duende.IdentityServer.Configuration;

namespace Aguacongas.IdentityServer.Admin.Services;

/// <summary>
/// 
/// </summary>
/// <seealso cref="JwtRequestValidator" />
/// <remarks>
/// Initializes a new instance of the <see cref="CustomJwtRequestValidator" /> class.
/// </remarks>
/// <param name="tokenValidationOptions">The token validation options.</param>
/// <param name="issuerNameService">The issuer name service.</param>
/// <param name="options">The options.</param>
/// <param name="logger">The logger.</param>
/// <exception cref="ArgumentNullException">tokenValidationOptions</exception>
public class CustomJwtRequestValidator(IOptions<TokenValidationParameters> tokenValidationOptions,
    IsConfiguration.IdentityServerOptions options,
    IIssuerNameService issuerNameService,
    ILogger<CustomJwtRequestValidator> logger) : JwtRequestValidator(options, issuerNameService, logger)
{
    private readonly TokenValidationParameters _tokenValidationOptions = tokenValidationOptions?.Value ?? throw new ArgumentNullException(nameof(tokenValidationOptions));

    /// <summary>
    /// Validates the JWT token
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="keys">The keys.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected override async Task<JsonWebToken> ValidateJwtAsync(JwtRequestValidationContext context, IEnumerable<SecurityKey> keys, CancellationToken ct)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKeys = keys,
            ValidIssuer = context.Client.ClientId,
            ValidAudience = await GetAudienceUri(ct).ConfigureAwait(false),
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
            tokenValidationParameters.ValidTypes = [JwtClaimTypes.JwtTypes.AuthorizationRequest];
        }

        var result = await Handler.ValidateTokenAsync(context.JwtTokenString, tokenValidationParameters).ConfigureAwait(false);
        if (!result.IsValid)
        {
            throw result.Exception;
        }

        return (JsonWebToken)result.SecurityToken;
    }
}