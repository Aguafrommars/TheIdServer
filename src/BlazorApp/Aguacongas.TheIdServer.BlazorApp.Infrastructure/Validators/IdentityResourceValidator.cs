using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class IdentityResourceValidator : AbstractValidator<IdentityResource>
    {
        public IdentityResourceValidator(IdentityResource identity)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("The id is required.");
            RuleFor(m => m.DisplayName).NotEmpty().WithMessage("The display name is required.");
            RuleFor(m => m.DisplayName).MaximumLength(200).WithMessage("The display name cannot exceed 200 chars.");
            RuleFor(m => m.Description).MaximumLength(2000).WithMessage("The description cannot exceed 2000 chars.");
            RuleForEach(m => m.IdentityClaims)
                .Where(m => m.Type != null)
                .SetValidator(new IdentityClaimValidator(identity));
            RuleFor(m => m.IdentityClaims).Must(c => c.Any(claim => !string.IsNullOrEmpty(claim.Type)))
                .WithMessage("The identity should provide at least one claim.");
            RuleForEach(m => m.Properties).SetValidator(new IdentityProrpertyValidator(identity));
        }
    }
}
