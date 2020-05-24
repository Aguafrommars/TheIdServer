using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Aguacongas.IdentityServer.Store.Entity;
using Models = Aguacongas.TheIdServer.BlazorApp.Models;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ExternalClaimTransformationValidator : AbstractValidator<ExternalClaimTransformation>
    {
        public ExternalClaimTransformationValidator(Models.ExternalProvider externalProvider)
        {
            RuleFor(m => m.FromClaimType).NotEmpty().WithMessage($"The from claim tyoe is required.");
            RuleFor(m => m.ToClaimType).NotEmpty().WithMessage($"The to claim tyoe is required.");
            RuleFor(m => m.FromClaimType).IsUnique(externalProvider.ClaimTransformations).WithMessage("The from claim type must be unique.");
        }
    }
}