// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation.Validation;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    public class WsFederationService : IWsFederationService
    {
        private readonly IUserSession _userSession;
        private readonly ISignInResponseGenerator _generator;
        private readonly ILogger<WsFederationController> _logger;
        private readonly IdentityServer4.Configuration.IdentityServerOptions _options;
        private readonly ISignInValidator _signinValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WsFederationService"/> class.
        /// </summary>
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
        public WsFederationService(ISignInValidator signinValidator,
            IdentityServer4.Configuration.IdentityServerOptions options,
            ISignInResponseGenerator generator,
            IUserSession userSession,
            ILogger<WsFederationController> logger)
        {
            _signinValidator = signinValidator ?? throw new ArgumentNullException(nameof(signinValidator));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public async Task<IActionResult> ProcessRequestAsync(HttpRequest request, IUrlHelper helper)
        {
            var queryString = request.QueryString;

            var user = await _userSession.GetUserAsync();
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
                _logger.LogDebug("User in WS-Federation signin request: {subjectId}", user.GetSubjectId());
            }
            else
            {
                _logger.LogDebug("No user present in WS-Federation signin request");
            }

            // validate request
            var result = await _signinValidator.ValidateAsync(signin, user);

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
                var responseMessage = await _generator.GenerateResponseAsync(result).ConfigureAwait(false);
                await _userSession.AddClientIdAsync(result.Client.ClientId);

                return new SignInResult(responseMessage);
            }
        }

        private static string AddQueryString(string url, string query)
        {
            if (!url.Contains("?"))
            {
                if (!query.StartsWith("?"))
                {
                    url += "?";
                }
            }
            else if (!url.EndsWith("&"))
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
}
