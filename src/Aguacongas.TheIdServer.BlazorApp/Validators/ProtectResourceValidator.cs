using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ProtectResourceValidator : AbstractValidator<ProtectResource>
    {
        public ProtectResourceValidator(ProtectResource api)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("The id is required.");
            RuleFor(m => m.DisplayName).NotEmpty().WithMessage("The name is required.");
            RuleFor(m => m.DisplayName).MaximumLength(200).WithMessage("The name cannot exceed 200 chars.");
            RuleFor(m => m.Description).MaximumLength(1000).WithMessage("The description cannot exceed 200 chars.");
            RuleForEach(m => m.Secrets).SetValidator(new ApiSecretValidator(api));
            RuleForEach(m => m.Scopes).SetValidator(new ApiScopeValidator(api));
            RuleForEach(m => m.Properties).SetValidator(new ApiPropertyValidator(api));
            RuleForEach(m => m.ApiClaims)
                .Where(m => m.Type != null)
                .SetValidator(new ApiClaimValidator(api));
        }
    }
}
