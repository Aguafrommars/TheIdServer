// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ProtectResourceValidator : AbstractValidator<ProtectResource>
    {
        public ProtectResourceValidator(ProtectResource api, IStringLocalizer localizer)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage(localizer["The id is required."]);
            RuleFor(m => m.DisplayName).NotEmpty().WithMessage(localizer["The name is required."]);
            RuleFor(m => m.DisplayName).MaximumLength(200).WithMessage(localizer["The name cannot exceed 200 chars."]);
            RuleFor(m => m.Description).MaximumLength(1000).WithMessage(localizer["The description cannot exceed 200 chars."]);
            RuleForEach(m => m.Secrets).SetValidator(new ApiSecretValidator(api, localizer));
            RuleForEach(m => m.Properties).SetValidator(new ApiPropertyValidator(api, localizer));
            RuleForEach(m => m.ApiClaims)
                .Where(m => m.Type != null)
                .SetValidator(new ApiClaimValidator(api, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.Description)
                .SetValidator(new EntityResourceValidator<ApiLocalizedResource>(api, EntityResourceKind.Description, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.DisplayName)
                .SetValidator(new EntityResourceValidator<ApiLocalizedResource>(api, EntityResourceKind.DisplayName, localizer));
        }
    }
}
