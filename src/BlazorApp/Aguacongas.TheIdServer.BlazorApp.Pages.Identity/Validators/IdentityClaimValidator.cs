// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class IdentityClaimValidator : AbstractValidator<IdentityClaim>
    {
        public IdentityClaimValidator(IdentityResource identity, IStringLocalizer localizer)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage(localizer["The identity claim type is required."]);
            RuleFor(m => m.Type).MaximumLength(250).WithMessage(localizer["The identity claim type cannot exceed 2000 chars."]);
            RuleFor(m => m.Type).IsUnique(identity.IdentityClaims).WithMessage(localizer["The identity claim type must be unique."]);
        }
    }
}