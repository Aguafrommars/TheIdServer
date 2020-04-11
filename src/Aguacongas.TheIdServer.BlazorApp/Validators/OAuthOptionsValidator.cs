using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class OAuthOptionsValidator : AbstractValidator<OAuthOptions>
    {
        public OAuthOptionsValidator(ExternalProvider _)
        {
            RuleFor(m => m.ClientId).NotEmpty().WithMessage("Client Id is required.");
            RuleFor(m => m.AuthorizationEndpoint).NotEmpty().WithMessage("Authorization endpoint is required.");
            RuleFor(m => m.AuthorizationEndpoint).Uri().WithMessage("Authorization endpoint must be a valid uri.");
            RuleFor(m => m.TokenEndpoint).NotEmpty().WithMessage("Token endpoint is required.");
            RuleFor(m => m.TokenEndpoint).Uri().WithMessage("Token endpoint must be a valid uri.");
            RuleFor(m => m.UserInformationEndpoint).NotEmpty().WithMessage("User information endpoint is required.");
            RuleFor(m => m.UserInformationEndpoint).Uri().WithMessage("User information endpoint must be a valid uri.");
        }
    }
}