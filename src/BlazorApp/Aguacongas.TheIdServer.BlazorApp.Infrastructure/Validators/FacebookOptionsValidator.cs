using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class FacebookOptionsValidator : AbstractValidator<FacebookOptions>
    {
        public FacebookOptionsValidator(ExternalProvider _)
        {
            RuleFor(m => m.AppId).NotEmpty().WithMessage("Client Id is required.");
            RuleFor(m => m.AppSecret).NotEmpty().WithMessage("Client Secret is required.");
        }
    }
}