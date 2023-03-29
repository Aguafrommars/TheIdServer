using Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Services;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services;
public class Saml2PService : ISaml2PService
{
    private readonly ISignInValidator _signInValidator;
    private readonly IUserSession _userSession;
    private readonly ISignInResponseGenerator _generator;
    private readonly IOptions<IdentityServerOptions> _identityServerOptions;
    private readonly ILogger<Saml2PService> _logger;

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

    public Task<IActionResult> ArtifactAsync(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IActionResult> LoginAsync(HttpRequest request, IUrlHelper helper)
    {
        
        var user = await _userSession.GetUserAsync().ConfigureAwait(false);

        var signinResult = await _signInValidator.ValidateAsync(request, user).ConfigureAwait(false);

        if (signinResult.Error is not null)
        {
            _logger.LogError(signinResult.ErrorMessage);
            return await _generator.GenerateLoginResponseAsync(signinResult, Saml2StatusCodes.Responder).ConfigureAwait(false);
        }

        if (signinResult.SignInRequired)
        {
            var returnUrl = helper.Action(nameof(AuthController.Login));
            returnUrl = AddQueryString(returnUrl, request.QueryString.Value);

            var userInteraction = _identityServerOptions.Value.UserInteraction;
            var loginUrl = request.PathBase + userInteraction.LoginUrl;
            var url = AddQueryString(loginUrl, userInteraction.LoginReturnUrlParameter, returnUrl);

            return new RedirectResult(url);
        }

        try
        {
            signinResult.Saml2RedirectBinding?.Unbind(signinResult.GerericRequest, signinResult.Saml2Request);

            return await _generator.GenerateLoginResponseAsync(signinResult, Saml2StatusCodes.Success).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, exc.Message);
            return await _generator.GenerateLoginResponseAsync(signinResult, Saml2StatusCodes.Responder).ConfigureAwait(false);
        }
    }

    public Task<IActionResult> LogoutAsync(HttpRequest request)
    {
        throw new NotImplementedException();
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
