// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class ApiScopeValidator : AbstractValidator<ApiScope>
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Our validators requires a parameter constructor.")]
        public ApiScopeValidator(ApiScope apiScope)
        {
            RuleForEach(m => m.Id).NotEmpty().WithMessage("The scope is required.");
            RuleFor(m => m.DisplayName).NotEmpty().WithMessage("The api scope display name is required.");
            RuleFor(m => m.DisplayName).MaximumLength(200).WithMessage("The api scope display name cannot exceed 200 chars.");
            RuleFor(m => m.Description).MaximumLength(2000).WithMessage("The api scope description cannot exceed 2000 chars.");
            RuleForEach(m => m.ApiScopeClaims)
                .Where(m => m.Type != null)
                .SetValidator(m => new ApiScopeClaimValidator(m));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.DisplayName)
                .SetValidator(m => new EntityResourceValidator<ApiScopeLocalizedResource>(m, EntityResourceKind.DisplayName));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.Description)
                .SetValidator(m => new EntityResourceValidator<ApiScopeLocalizedResource>(m, EntityResourceKind.Description));
        }
    }
}