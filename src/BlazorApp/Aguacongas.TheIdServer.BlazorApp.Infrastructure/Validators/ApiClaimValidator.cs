using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ApiClaimValidator : AbstractValidator<ApiClaim>
    {
        public ApiClaimValidator(ProtectResource api)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("The api claim type is required.");
            RuleFor(m => m.Type).MaximumLength(250).WithMessage("The api claim type cannot exceed 2000 chars.");
            RuleFor(m => m.Type).IsUnique(api.ApiClaims).WithMessage("The api claim type must be unique.");
        }
    }
}