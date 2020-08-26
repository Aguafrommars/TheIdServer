// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class ApiScopeClaimValidator : AbstractValidator<ApiScopeClaim>
    {
        public ApiScopeClaimValidator(ApiScope scope)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("The scope claim type is required.");
            RuleFor(m => m.Type).MaximumLength(250).WithMessage("The scope claim type cannot exceed 2000 chars.");
            RuleFor(m => m.Type).IsUnique(scope.ApiScopeClaims).WithMessage("The scope claim type must be unique.");
        }
    }
}