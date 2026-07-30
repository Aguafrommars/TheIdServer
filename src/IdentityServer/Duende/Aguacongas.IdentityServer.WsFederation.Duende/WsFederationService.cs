// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Validation;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Configuration = Duende.IdentityServer.Configuration;

namespace Aguacongas.IdentityServer.WsFederation;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WsFederationService"/> class.
/// </remarks>
/// <param name="signinValidator">The signin validator.</param>
/// <param name="options">The options.</param>
/// <param name="generator">The generator.</param>
/// <param name="userSession">The user session.</param>
/// <param name="logger">The logger.</param>
/// <exception cref="ArgumentNullException">
/// signinValidator
/// or
/// options
/// or
/// generator
/// or
/// userSession
/// or
/// logger
/// </exception>
public class WsFederationService(ISignInValidator signinValidator,
    Configuration.IdentityServerOptions options,
    ISignInResponseGenerator generator,
    IUserSession userSession,
    ILogger<WsFederationService> logger) : IWsFederationService
{
    private readonly IUserSession _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    private readonly ISignInResponseGenerator _generator = generator ?? throw new ArgumentNullException(nameof(generator));
    private readonly ILogger<WsFederationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Configuration.IdentityServerOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly ISignInValidator _signinValidator = signinValidator ?? throw new ArgumentNullException(nameof(signinValidator));

    /// <summary>
    /// Processes the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="helper">The helper.</param>
    /// <returns></returns>
    public async Task<IActionResult> ProcessRequestAsync(HttpRequest request, IUrlHelper helper)
    {
        var queryString = request.QueryString;

        var user = await _userSession.GetUserAsync(request.HttpContext.RequestAborted).ConfigureAwait(false);
        var message = WsFederationMessage.FromQueryString(queryString.ToString());

        if (message.IsSignInMessage)
        {
            return await ProcessSignInAsync(message, user, request, helper).ConfigureAwait(false);
        }

        if (message.IsSignOutMessage)
        {
            return new RedirectResult($"~/connect/endsession{queryString}");
        }

        return new BadRequestObjectResult("Invalid WS-Federation request");
    }

    private async Task<IActionResult> ProcessSignInAsync(WsFederationMessage signin, ClaimsPrincipal user, HttpRequest request, IUrlHelper helper)
    {
        if (user != null && user.Identity.IsAuthenticated)
        {
            _logger.LogDebug("User in WS-Federation signin request: {SubjectId}", user.GetSubjectId());
        }
        else
        {
            _logger.LogDebug("No user present in WS-Federation signin request");
        }

        // validate request
        var result = await _signinValidator.ValidateAsync(signin, user, request.HttpContext.RequestAborted).ConfigureAwait(false);

        if (result.IsError)
        {
            return new BadRequestObjectResult(result);
        }

        if (result.SignInRequired)
        {
            var returnUrl = helper.Action(nameof(WsFederationController.Index));
            returnUrl = AddQueryString(returnUrl, request.QueryString.Value);

            var loginUrl = request.PathBase + _options.UserInteraction.LoginUrl;
            var url = AddQueryString(loginUrl, _options.UserInteraction.LoginReturnUrlParameter, returnUrl);

            return new RedirectResult(url);
        }
        else
        {
            // create protocol response
            var responseMessage = await _generator.GenerateResponseAsync(result, request.HttpContext.RequestAborted).ConfigureAwait(false);
            await _userSession.AddClientIdAsync(result.Client.ClientId, request.HttpContext.RequestAborted).ConfigureAwait(false);

            return new SignInResult(responseMessage);
        }
    }

    private static string AddQueryString(string url, string query)
    {
        if (!url.Contains('?'))
        {
            if (!query.StartsWith('?'))
            {
                url += "?";
            }
        }
        else if (!url.EndsWith('&'))
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