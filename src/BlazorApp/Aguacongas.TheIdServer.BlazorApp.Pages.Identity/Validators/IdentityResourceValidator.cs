// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class IdentityResourceValidator : AbstractValidator<IdentityResource>
    {
        public IdentityResourceValidator(IdentityResource identity, IStringLocalizer localizer)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage(localizer["The id is required."]);
            RuleFor(m => m.DisplayName).NotEmpty().WithMessage(localizer["The display name is required."]);
            RuleFor(m => m.DisplayName).MaximumLength(200).WithMessage(localizer["The display name cannot exceed 200 chars."]);
            RuleFor(m => m.Description).MaximumLength(2000).WithMessage(localizer["The description cannot exceed 2000 chars."]);
            RuleForEach(m => m.IdentityClaims)
                .Where(m => m.Type != null)
                .SetValidator(new IdentityClaimValidator(identity, localizer));
            RuleFor(m => m.IdentityClaims).Must(c => c.Any(claim => !string.IsNullOrEmpty(claim.Type)))
                .WithMessage(localizer["The identity should provide at least one claim."]);
            RuleForEach(m => m.Properties).SetValidator(new IdentityProrpertyValidator(identity, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.DisplayName)
                .SetValidator(new EntityResourceValidator<IdentityLocalizedResource>(identity, EntityResourceKind.DisplayName, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.Description)
                .SetValidator(new EntityResourceValidator<IdentityLocalizedResource>(identity, EntityResourceKind.Description, localizer));
        }
    }
}
