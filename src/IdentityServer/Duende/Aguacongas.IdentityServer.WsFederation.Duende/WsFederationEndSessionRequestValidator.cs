// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Validation;
using Duende.IdentityServer.WsFederation;
using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IEndSessionRequestValidator" />
    public class WsFederationEndSessionRequestValidator : IEndSessionRequestValidator
    {
        private readonly EndSessionRequestValidator _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="WsFederationEndSessionRequestValidator"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <exception cref="ArgumentNullException">parent</exception>
        public WsFederationEndSessionRequestValidator(EndSessionRequestValidator parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Validates end session endpoint requests.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task<EndSessionValidationResult> ValidateAsync(NameValueCollection parameters, ClaimsPrincipal subject)
        {
            var result = await _parent.ValidateAsync(parameters, subject).ConfigureAwait(false);
            var redirectUri = parameters.Get(WsFederationConstants.Wreply);
            if (!string.IsNullOrEmpty(redirectUri))
            {
                result.ValidatedRequest.PostLogOutUri = redirectUri;
            }
            return result;
        }

        /// <summary>
        /// Validates requests from logout page iframe to trigger single signout.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<EndSessionCallbackValidationResult> ValidateCallbackAsync(NameValueCollection parameters)
        => _parent.ValidateCallbackAsync(parameters);
    }
}
