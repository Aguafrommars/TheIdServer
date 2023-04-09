using Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Services;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services;

/// <summary>
/// Saml2P service
/// </summary>
public class Saml2PService : ISaml2PService
{
    private readonly ISignInValidator _signInValidator;
    private readonly IUserSession _userSession;
    private readonly ISignInResponseGenerator _generator;
    private readonly IOptions<IdentityServerOptions> _identityServerOptions;
    private readonly ILogger<Saml2PService> _logger;

    /// <summary>
    /// Initialize a new instance of <see cref="Saml2PService"/>
    /// </summary>
    /// <param name="signInValidator"></param>
    /// <param name="userSession"></param>
    /// <param name="generator"></param>
    /// <param name="identityServerOptions"></param>
    /// <param name="logger"></param>
    public Saml2PService(ISignInValidator signInValidator,
        IUserSession userSession,
        ISignInResponseGenerator generator,
        IOptions<IdentityServerOptions> identityServerOptions,
        ILogger<Saml2PService> logger)
    {
        _signInValidator = signInValidator;
        _userSession = userSession;
        _generator = generator;
        _identityServerOptions = identityServerOptions;
        _logger = logger;
    }

    /// <summary>
    /// Handles artifact request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> ArtifactAsync(HttpRequest request)
    {
        var result = await _signInValidator.ValidateArtifactRequestAsync(request).ConfigureAwait(false);
        if (result.Error is not null)
        {
            return new BadRequestObjectResult(result.Error);
        }

        return await _generator.GenerateArtifactResponseAsync(result).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles login request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="helper"></param>
    /// <returns></returns>
    public async Task<IActionResult> LoginAsync(HttpRequest request, IUrlHelper helper)
    {
        var user = await _userSession.GetUserAsync().ConfigureAwait(false);

        var signinResult = await _signInValidator.ValidateLoginAsync(request, user).ConfigureAwait(false);

        if (signinResult.Error is not null)
        {
            _logger.LogError("{Error}", signinResult.ErrorMessage);
            return await _generator.GenerateLoginResponseAsync(signinResult, Saml2StatusCodes.Responder).ConfigureAwait(false);
        }

        if (signinResult.SignInRequired)
        {
            var returnUrl = helper.Action(nameof(Saml2PController.Login));
            returnUrl = AddQueryString(returnUrl, request.QueryString.Value);

            var userInteraction = _identityServerOptions.Value.UserInteraction;
            var loginUrl = request.PathBase + userInteraction.LoginUrl;
            var url = AddQueryString(loginUrl, userInteraction.LoginReturnUrlParameter, returnUrl);

            return new RedirectResult(url);
        }

        try
        {
            signinResult.Saml2Binding?.Unbind(signinResult.GerericRequest, signinResult.Saml2Request);

            return await _generator.GenerateLoginResponseAsync(signinResult, Saml2StatusCodes.Success).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{Message}", exc.Message);
            return await _generator.GenerateLoginResponseAsync(signinResult, Saml2StatusCodes.Responder).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handles logout request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> LogoutAsync(HttpRequest request)
    {
        var signinResult = await _signInValidator.ValidateLogoutAsync(request).ConfigureAwait(false);
        try
        {
            signinResult.Saml2Binding?.Unbind(signinResult.GerericRequest, signinResult.Saml2Request);

            return await _generator.GenerateLogoutResponseAsync(signinResult, Saml2StatusCodes.Success).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{Message}", exc.Message);
            return await _generator.GenerateLogoutResponseAsync(signinResult, Saml2StatusCodes.Responder).ConfigureAwait(false);
        }
    }

    private static string AddQueryString(string? url, string? query)
    {
        if (url?.Contains('?') == false)
        {
            if (query?.StartsWith("?") == false)
            {
                url += "?";
            }
        }
        else if (url?.EndsWith("&") == false)
        {
            url += "&";
        }

        return url + query;
    }

    private static string AddQueryString(string url, string name, string value)
    {
        return AddQueryString(url, $"{name}={UrlEncoder.Default.Encode(value)}");
    }
}
