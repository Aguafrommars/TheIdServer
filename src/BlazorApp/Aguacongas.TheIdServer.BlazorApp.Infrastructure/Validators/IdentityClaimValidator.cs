using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class IdentityClaimValidator : AbstractValidator<IdentityClaim>
    {
        public IdentityClaimValidator(IdentityResource identity)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("The identity claim type is required.");
            RuleFor(m => m.Type).MaximumLength(250).WithMessage("The identity claim type cannot exceed 2000 chars.");
            RuleFor(m => m.Type).IsUnique(identity.IdentityClaims).WithMessage("The identity claim type must be unique.");
        }
    }
}