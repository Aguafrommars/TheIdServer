// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Validation;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ClaimProperties = Microsoft.IdentityModel.Tokens.Saml.ClaimProperties;
using ISValidation = Duende.IdentityServer.Validation;
using IsWsFederationConstant = Duende.IdentityServer.WsFederation.WsFederationConstants;

namespace Aguacongas.IdentityServer.WsFederation;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SignInResponseGenerator"/> class.
/// </remarks>
/// <param name="issuerNameService">The <see cref="IIssuerNameService"/>.</param>
/// <param name="profile">The profile.</param>
/// <param name="keys">The keys.</param>
/// <param name="resources">The resources.</param>
/// <param name="logger">The logger.</param>
public class SignInResponseGenerator(
    IIssuerNameService issuerNameService,
    IProfileService profile,
    ISigningCredentialStore keys,
    IResourceStore resources,
    ILogger<SignInResponseGenerator> logger) : ISignInResponseGenerator
{
    private readonly IIssuerNameService _issuerNameService = issuerNameService ?? throw new ArgumentNullException(nameof(issuerNameService));
    private readonly IProfileService _profile = profile ?? throw new ArgumentNullException(nameof(profile));
    private readonly ISigningCredentialStore _keys = keys ?? throw new ArgumentNullException(nameof(keys));
    private readonly IResourceStore _resources = resources ?? throw new ArgumentNullException(nameof(resources));
    private readonly ILogger<SignInResponseGenerator> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Generates the response asynchronous.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task<WsFederationMessage> GenerateResponseAsync(SignInValidationResult validationResult, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating WS-Federation signin response");

        // create subject
        var outgoingSubject = await CreateSubjectAsync(validationResult, cancellationToken).ConfigureAwait(false);

        // create token for user
        var token = await CreateSecurityTokenAsync(validationResult, outgoingSubject, cancellationToken).ConfigureAwait(false);
        // return response
        return CreateResponse(validationResult, token);
    }

    /// <summary>
    /// Creates the subject asynchronous.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    protected async Task<ClaimsIdentity> CreateSubjectAsync(SignInValidationResult result, CancellationToken cancellationToken)
    {
        var requestedClaimTypes = new List<string>();

        var identityResources = await _resources.FindEnabledIdentityResourcesByScopeAsync(result.Client.AllowedScopes, cancellationToken).ConfigureAwait(false);
        foreach (var resource in identityResources)
        {
            foreach (var claim in resource.UserClaims)
            {
                requestedClaimTypes.Add(claim);
            }
        }

        var client = result.Client;
        var ctx = new ProfileDataRequestContext
        {
            Subject = result.User,
            RequestedClaimTypes = requestedClaimTypes,
            Application = client,
            Caller = "WS-Federation",
            RequestedResources = new ISValidation.ResourceValidationResult
            {
                Resources = new Resources
                {
                    IdentityResources = [.. identityResources]
                }
            }
        };

        await _profile.GetProfileDataAsync(ctx, cancellationToken).ConfigureAwait(false);

        ctx.IssuedClaims.AddRange(client.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)));
        // map outbound claims
        var relyParty = result.RelyingParty;
        var mapping = relyParty.ClaimMapping;

        var nameid = new Claim(ClaimTypes.NameIdentifier, result.User.GetSubjectId());
        nameid.Properties[ClaimProperties.SamlNameIdentifierFormat] = relyParty.SamlNameIdentifierFormat;

        var outboundClaims = new List<Claim> { nameid };
        foreach (var claim in ctx.IssuedClaims)
        {
            if (mapping.ContainsKey(claim.Type))
            {
                AddMappedClaim(relyParty, outboundClaims, mapping, claim);
            }
            else if (Uri.TryCreate(claim.Type, UriKind.Absolute, out Uri _) ||
                relyParty.TokenType != IsWsFederationConstant.TokenTypes.Saml11TokenProfile11)
            {
                outboundClaims.Add(claim);
            }
            else
            {
                _logger.LogInformation("No explicit claim type mapping for {ClaimType} configured. Saml requires a URI claim type. Skipping.", claim.Type);
            }
        }

        // The AuthnStatement statement generated from the following 2
        // claims is manditory for some service providers (i.e. Shibboleth-Sp). 
        // The value of the AuthenticationMethod claim must be one of the constants in
        // System.IdentityModel.Tokens.AuthenticationMethods.
        // Password is the only one that can be directly matched, everything
        // else defaults to Unspecified.
        if (result.User.GetAuthenticationMethod() == OidcConstants.AuthenticationMethods.Password)
        {
            outboundClaims.Add(new Claim(ClaimTypes.AuthenticationMethod, SamlConstants.AuthenticationMethods.PasswordString));
        }
        else
        {
            outboundClaims.Add(new Claim(ClaimTypes.AuthenticationMethod, SamlConstants.AuthenticationMethods.UnspecifiedString));
        }

        // authentication instant claim is required
        outboundClaims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.UtcNow, "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime));

        return new ClaimsIdentity(outboundClaims, "theidserver");
    }

    private static void AddMappedClaim(Stores.RelyingParty relyParty, List<Claim> outboundClaims, IDictionary<string, string> mapping, Claim claim)
    {
        var outboundClaim = new Claim(mapping[claim.Type], claim.Value, claim.ValueType);
        if (outboundClaim.Type == ClaimTypes.NameIdentifier)
        {
            outboundClaim.Properties[ClaimProperties.SamlNameIdentifierFormat] = relyParty.SamlNameIdentifierFormat;
            outboundClaims.RemoveAll(c => c.Type == ClaimTypes.NameIdentifier); //remove previesly added nameid claim
        }
        if (outboundClaim.Type == ClaimTypes.Name)
        {
            outboundClaims.RemoveAll(c => c.Type == ClaimTypes.Name); //remove previesly added name claim
        }

        outboundClaims.Add(outboundClaim);
    }

    private async Task<SecurityToken> CreateSecurityTokenAsync(SignInValidationResult result, ClaimsIdentity outgoingSubject, CancellationToken cancellationToken)
    {
        var credentials = await _keys.GetSigningCredentialsAsync(cancellationToken).ConfigureAwait(false);
        var x509Key = new X509SecurityKey(credentials.Key.GetX509Certificate(_keys));

        var relyingParty = result.RelyingParty;
        var descriptor = new SecurityTokenDescriptor
        {
            Audience = result.Client.ClientId,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(result.Client.IdentityTokenLifetime),
            SigningCredentials = new SigningCredentials(x509Key, relyingParty.SignatureAlgorithm, relyingParty.DigestAlgorithm),
            Subject = outgoingSubject,
            Issuer = await _issuerNameService.GetCurrentAsync(cancellationToken).ConfigureAwait(false),
        };

        if (result.RelyingParty.EncryptionCertificate != null)
        {
            descriptor.EncryptingCredentials = new X509EncryptingCredentials(result.RelyingParty.EncryptionCertificate);
        }

        var handler = CreateTokenHandler(result.RelyingParty.TokenType);
        return handler.CreateToken(descriptor);
    }

    private static WsFederationMessage CreateResponse(SignInValidationResult validationResult, SecurityToken token)
    {
        var handler = CreateTokenHandler(validationResult.RelyingParty.TokenType);
        var client = validationResult.Client;
        var message = validationResult.WsFederationMessage;
        var rstr = new RequestSecurityTokenResponse
        {
            CreatedAt = token.ValidFrom,
            ExpiresAt = token.ValidTo,
            AppliesTo = client.ClientId,
            Context = message.Wctx,
            ReplyTo = validationResult.ReplyUrl,
            RequestedSecurityToken = token,
            SecurityTokenHandler = handler,
        };
        var responseMessage = new WsFederationMessage
        {
            IssuerAddress = message.Wreply ?? client.RedirectUris.First(),
            Wa = WsFederationConstants.WsFederationActions.SignIn,
            Wresult = rstr.Serialize(),
            Wctx = message.Wctx
        };
        return responseMessage;
    }

    private static SecurityTokenHandler CreateTokenHandler(string tokenType)
    {
        return tokenType switch
        {
            IsWsFederationConstant.TokenTypes.Saml11TokenProfile11 => new SamlSecurityTokenHandler(),
            IsWsFederationConstant.TokenTypes.Saml2TokenProfile11 => new Saml2SecurityTokenHandler(),
            _ => throw new NotImplementedException($"TokenType: {tokenType} not implemented"),
        };
    }
}