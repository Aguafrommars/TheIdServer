using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Validators
{
    public class BackchannelAuthenticationUserValidator : IBackchannelAuthenticationUserValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BackchannelAuthenticationUserValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<BackchannelAuthenticationUserValidatonResult> ValidateRequestAsync(BackchannelAuthenticationUserValidatorContext userValidatorContext)
        {
            var sub = userValidatorContext.LoginHintToken ?? userValidatorContext.IdTokenHintClaims?.SingleOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

            if (sub is null && userValidatorContext.LoginHint is null)
            {
                return new BackchannelAuthenticationUserValidatonResult
                {
                    Error = "Cannot get user.",
                    ErrorDescription = $"LoginHintToken, LoginHint and IdTokenHint don't contain user id."
                };
            }

            var user = userValidatorContext.LoginHint is not null ?
                await _userManager.FindByNameAsync(userValidatorContext.LoginHint).ConfigureAwait(false) :
                await _userManager.FindByIdAsync(sub).ConfigureAwait(false);

            if (user is null)
            {
                return new BackchannelAuthenticationUserValidatonResult
                {
                    Error = "User not found.",
                    ErrorDescription = $"LoginHintToken, LoginHint and IdTokenHint don't contain a valid user id."
                };
            }

            return new BackchannelAuthenticationUserValidatonResult
            {
                Subject = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(JwtClaimTypes.Subject, user.Id),
                    new Claim(JwtClaimTypes.Name, user.UserName),
                    new Claim(JwtClaimTypes.Email, user.Email)
                }, "ciba", JwtClaimTypes.Name, JwtClaimTypes.Role))
            };
        }
    }
}
