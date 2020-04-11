using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class OpenIdConnectOptionsValidator : AbstractValidator<OpenIdConnectOptions>
    {
        public OpenIdConnectOptionsValidator(ExternalProvider _)
        {
            RuleFor(m => m.Authority).NotEmpty().WithMessage("Authority is required.");
            RuleFor(m => m.Authority).Uri().WithMessage("Authority must be a valid uir.");
            RuleFor(m => m.ClientId).NotEmpty().WithMessage("Client Id is required.");
        }
    }
}